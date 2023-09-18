using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Agents
{
    internal class LLMAction : AgentAction
    {
        private readonly ILanguageModel LanguageModel;
        public LLMAction(ILanguageModel languageModel) : base(null) { LanguageModel = languageModel; }

        public ChatMessage SystemMessage { get; set; }
        public ChatMessage UserMessage { get; set; }
        public IEnumerable<FunctionDescriptor> Functions { get; set; }
        public double Temperature { get; set; }

        public override IEnumerable<string> Parameters => Enumerable.Empty<string>();

        public override async Task<string> ExecuteAsync()
        {
            var msges = new[] { SystemMessage, UserMessage };
            LLMResponse = await LanguageModel.GetResponseAsync(msges, Functions, Temperature);

            return LLMResponse.Response;
        }

        public LLMResponse LLMResponse { get; private set; } = new LLMResponse() { Type = ResponseType.Failed };
    }

    internal class GenerativeAIAgent
    {
        private ILanguageModel languageModel;
        private List<ChatMessage> messages = new List<ChatMessage>();
        private IResponseParser responseParser = new ChainOfThoughtResponsePraser();
        private ChatMessage systemprompt;

        private GenerativeAIAgent() { }

        private ToolsCollection functionTools = null;

        public static GenerativeAIAgent WithTools(IEnumerable<IFunctionTool> tools)
        {
            var agent = new GenerativeAIAgent() { functionTools = new ToolsCollection(tools) };
            agent.systemprompt = agent.responseParser.GetSystemPrompt(tools);
            return agent;
        }

        /// <summary>
        /// Sets language model to the tool
        /// </summary>
        /// <param name="model">The language model implementation</param>
        /// <returns>GenerativeAIAgent</returns>
        public GenerativeAIAgent WithLanguageModel(ILanguageModel model)
        {
            this.languageModel = model;
            return this;
        }

        private ILanguageModel LanguageModel
        {
            get
            {
                if (null == languageModel)
                {
                    languageModel = Application.DefaultLanguageModel;
                }

                return languageModel;
            }
        }

        private LLMAction llmAction = null;

        public async Task<string> PlanAndExecute(string objective, bool verbose = false)
        {
            var ctx = new ExecutionContext();
            ctx["objective"] = objective;

            FinishAction finishaction = null;

            while(finishaction == null)
            {
                var action = await GetNextActionAsync(ctx);
                
                var observation = await action.ExecuteAsync();
                responseParser.AppendObservation(observation);
                finishaction = action as FinishAction;
            }

            if (verbose) return responseParser.ScratchPad;

            return finishaction.Output;
        }

        protected async Task<AgentAction> GetNextActionAsync(ExecutionContext context)
        {
            var temperature = 0.8;
            var objective = context["objective"];
            var usermsg = responseParser.GetUserPrompt((string)objective);
            List<ChatMessage> msgs = new List<ChatMessage>() { systemprompt, usermsg };
            LLMResponse response = new LLMResponse() { Type = ResponseType.Failed };
            response = await languageModel.GetResponseAsync(msgs, functionTools.GetFunctions(), temperature);

            var action = responseParser.ParseResponse(response);
            while(action == null)
            {
                usermsg = responseParser.GetUserPrompt((string)objective);
                msgs = new List<ChatMessage>() { systemprompt, usermsg };
                response = await languageModel.GetResponseAsync(msgs, functionTools.GetFunctions(), temperature);
                action = responseParser.ParseResponse(response);
            }

            return action;
        }

        protected string FinishToolName => "Final Answer";

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
    }
}
