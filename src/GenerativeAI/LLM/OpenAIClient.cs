using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Stores;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.LLM
{
    internal class OpenAIClient : ILanguageModel
    {
        /// <summary>
        /// Name of the model used within OpenAI
        /// </summary>
        public string ModelName => config.Model;

        public IVectorTransformer VectorTransformer => transformer;

        private int promptTokens = 0;
        public int PromptTokensUsed => promptTokens;

        private int completionTokens = 0;
        public int CompletionTokensUsed => completionTokens;

        public int MaxTokenLimit => config.TokenLimit;

        private readonly IVectorTransformer transformer;
        private readonly OpenAIConfig config;
        private readonly HttpTool httpTool;
        
        public OpenAIClient(OpenAIConfig config)
        {
            this.config = config;
            var headers = new Dictionary<string, string>();
            if (config.AzureConfig)
            {
                headers["api-key"] = config.ApiKey;
            }
            else
            {
                headers["Authorization"] = $"Bearer {config.ApiKey}";
            }
            httpTool = HttpTool.WithClient().WithDefaultRequestHeaders(headers);
            transformer = new OpenAIEmbeddingTransformer();
        }

        private static LLMResponse ToLLMResponse(ChatResponse response)
        {
            var msg = response.choices[0].message;
            var finish_reason = response.choices[0].finish_reason;
            switch (finish_reason.ToLower())
            {
                case "stop":
                    return new LLMResponse() { Type = ResponseType.Done, Response = msg.content };
                case "function_call":
                    var serializer = new JavaScriptSerializer();
                    string json = serializer.Serialize(msg.function_call);
                    return new LLMResponse() { Type = ResponseType.FunctionCall, Response = json };
                default:
                    return new LLMResponse() { Type = ResponseType.Partial, Response = msg.content };
            }
        }

        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
        {
            return await GetResponseAsync(messages, null, temperature);
        }

        internal static List<Dictionary<string, object>> ToFunctions(IEnumerable<FunctionDescriptor> functions)
        {
            var funcs = new List<Dictionary<string, object>>();
            foreach (var item in functions)
            {
                var parameters = item.Parameters.ToDictionary();
                var function = new Dictionary<string, object>
                {
                    { "name", item.Name },
                    { "description", item.Description },
                    { "parameters", parameters }
                };

                funcs.Add(function);
            }

            return funcs;
        }

        internal static string ToJSON(IEnumerable<FunctionDescriptor> functions)
        {
            var funcs = ToFunctions(functions);

            var serializer = new JavaScriptSerializer();

            string json = serializer.Serialize(funcs);
            return json;
        }

        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            try
            {
                ChatRequest data = null;
                if (functions == null || !functions.Any())
                {
                    data = new ChatRequest()
                    {
                        model = config.Model,
                        messages = messages.ToArray(),
                        temperature = temperature
                    };
                }
                else
                {
                    var funcs = ToFunctions(functions);
                    data = new FunctionRequest()
                    {
                        model = config.Model,
                        messages = messages.ToArray(),
                        temperature = temperature,
                        functions = funcs
                    };
                }

                var serializer = new JavaScriptSerializer();

                string jsonPayload = serializer.Serialize(data);
                Logger.WriteLog(LogLevel.Info, LogOps.Request, jsonPayload);

                string json = await httpTool.PostAsync(config.CompletionsUrl, jsonPayload);

                var response = serializer.Deserialize<ChatResponse>(json);
                
                //update usage tokens
                Interlocked.Add(ref promptTokens, response.usage.prompt_tokens);
                Interlocked.Add(ref completionTokens, response.usage.completion_tokens);

                var llmResponse = ToLLMResponse(response);

                //Log info
                Logger.WriteLog(LogLevel.Info, LogOps.Response, llmResponse.Response);
                Logger.WriteLog(LogLevel.Info, LogOps.Response, $"Prompts Tokens: {response.usage.prompt_tokens}, Completion Tokens: {response.usage.completion_tokens}");
                
                return llmResponse; 
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.Message);
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.StackTrace);
                return new LLMResponse() { Response = ex.Message, Type = ResponseType.Failed };
            }
        }
    }
}
