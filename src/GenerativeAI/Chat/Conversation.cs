using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Stores;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Chat
{
    internal class Conversation : IConversation
    {
        private readonly string contextid;
        private readonly List<ChatMessage> messages = new List<ChatMessage>();
        private readonly ILanguageModel languageModel;
        private IVectorStore store = null;
        private IFunctionToolSet tool = null;
        private PromptTemplate contextPrompt = null;
        private ChatMessage systemMessage = null;

        public IEnumerable<ChatMessage> GetChatHistory() { return messages; }

        public Conversation(string id, ILanguageModel llm)
        {
            contextid = id;
            languageModel = llm;
            var template = @" Use the following pieces of context to answer the question at the end.If you don't know the answer, just say that you don't know, don't try to make up an answer.
CONTEXT:
{{$context}}

QUESTION:
{{$question}}
";

            contextPrompt = new PromptTemplate(template);
        }

        public string Id => contextid;

        public void AppendMessage(string message, Role role)
        {
            var chatmessage = new ChatMessage(role, message);
            if (role == Role.system) 
            {
                systemMessage = chatmessage;
            }
            else
            {
                messages.Add(chatmessage);
            }
        }

        public void AppendMessage(ChatMessage message)
        {
            if(message.role == "system")
            {
                systemMessage = message;
            }
            else
            {
                messages.Add(message);
            }
        }

        public void AddContext(IEnumerable<ITextObject> context)
        {
            if(store == null)
            {
                store = new VectorStore(languageModel.VectorTransformer);
            }

            store.Add(context, true);
        }

        public void AddContext(IVectorStore ctxstore)
        {
            store = ctxstore;
        }

        internal static ChatMessage MessgeFromResponse(LLMResponse response)
        {
            switch (response.Type)
            {
                case ResponseType.Failed:
                    break;
                case ResponseType.Done:
                    return new ChatMessage(Role.assistant, response.Response);
                case ResponseType.Partial:
                    return new ChatMessage(Role.assistant, response.Response);
                case ResponseType.FunctionCall:
                    var serializer = new JavaScriptSerializer();
                    var function_call = serializer.Deserialize<Dictionary<string, object>>(response.Response);
                    return new FunctionCallMessage() { function_call = function_call };
                default:
                    break;
            }
            return null;
        }

        public async Task<ChatMessage> GetResponseAsync(double temperature)
        {
            var msgs = new List<ChatMessage>();

            //If there is any system message add that to the request messages
            if (systemMessage != null)
            {
                msgs.Add(systemMessage);
            }

            //If vector store context is available then perform semantic search to
            //identify relevant content as context
            if (store != null && messages.Count > 0)
            {
                var msg = messages.Last();
                Logger.WriteLog(LogLevel.Info, LogOps.Command, $"Query String: {msg.content}");
                var match = store.Search(TextObject.Create("Context", msg.content), 4).ToArray();
                string context = string.Empty;
                if (match.Length > 0)
                {
                    var pages = match.Select(x => x.Attributes["Index"]).Aggregate((x, y) => $"{x}, {y}");
                    Logger.WriteLog(LogLevel.Info, LogOps.Found, $"Context from VDB, Indices: {pages}");
                    context = string.Join(" ", match.Select(x => x.Attributes["Text"]).ToArray());
                    if (context.Length > 2)
                    {
                        var exectx = new ExecutionContext();
                        exectx["context"] = context;
                        exectx["question"] = msg.content;
                        msg = contextPrompt.FormatMessage(exectx);
                    }
                    else
                    {
                        Logger.WriteLog(LogLevel.Info, LogOps.NotFound, "Context from VDB");
                    }
                    msgs.Add(msg);
                }
                if (string.IsNullOrEmpty(context))
                {
                    //Didn't find the right context, so just include message history as context.
                    msgs.AddRange(messages);
                }
            }
            else
            {
                msgs.AddRange(messages);
            }

            var functions = Enumerable.Empty<FunctionDescriptor>();
            if (tool != null)
            {
                functions = tool.GetFunctions();
            }

            LLMResponse response = new LLMResponse() { Type = ResponseType.Failed };
            while (response.Type != ResponseType.Done)
            {
                response = await languageModel.GetResponseAsync(msgs, functions, temperature);
                //When there is no tool registered then one call is good
                if (response.Type == ResponseType.Failed) return new ChatMessage(Role.system, response.Response);

                var msg = MessgeFromResponse(response);
                msgs.Add(msg);

                if (tool != null && response.Type == ResponseType.FunctionCall)
                {
                    //Try to execute the message
                    var fmsg = msg as FunctionCallMessage;
                    object function = null;
                    fmsg.function_call.TryGetValue("name", out function);
                    string args = (string)fmsg.function_call["arguments"];

                    var serializer = new JavaScriptSerializer();
                    var arguments = serializer.Deserialize<Dictionary<string, object>>(args);
                    var context = new ExecutionContext(arguments);
                    var output = await tool.ExecuteAsync((string)function, context);

                    if (string.IsNullOrEmpty(output))
                    {
                        messages.Add(fmsg);
                        return fmsg;
                    }

                    msgs.Add(new FunctionMessage((string)function, output));
                }
            }
            if (response.Type == ResponseType.Done)
            {
                messages.Add(msgs.Last());
            }
            return messages.Last();
        }

        internal static ChatMessage GetFunctionCallSystemMessage()
        {
            return new ChatMessage(Role.system, $@"You are an intelligent assistant. Think step by step and analyze the input 
                  request to check if any function call is required, if so extract all
                  parameters based on the function sepcification. Extract arguments and values
                  only based on function specification provided, do not include extra parameter. 
                  Today's date: {DateTime.Today}");
        }

        public void AddToolSet(IFunctionToolSet toolSet)
        {
            //update system message to extract data using toolset
            systemMessage = GetFunctionCallSystemMessage();

            this.tool = toolSet;
        }
    }
}
