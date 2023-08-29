using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.LLM
{
    internal class ChatRequest
    {
        public string model { get; set; }

        public ChatMessage[] messages { get; set; }

        public double temperature { get; set; }
    }

    internal class FunctionRequest : ChatRequest
    {
        public List<Dictionary<string, object>> functions { get; set; }
    }

    internal class Choice
    {
        public int index { get; set; }
        public FunctionCallMessage message { get; set; }
        public string finish_reason { get; set; }
    }

    internal class ChatResponse
    {
        public string id { get; set; }
        public int created { get; set; }
        public Choice[] choices { get; set; }
    }

    /// <summary>
    /// OpenAI language model implementation
    /// </summary>
    public class OpenAILanguageModel : ILanguageModel
    {
        /// <summary>
        /// Name of the model used within OpenAI
        /// </summary>
        public string ModelName => model;

        private readonly string model;
        private readonly HttpTool httpTool;
        private static readonly string apiUrl = "https://api.openai.com/v1/chat/completions";

        /// <summary>
        /// Creates OpenAILanguageModel instance.
        /// </summary>
        /// <param name="model">Name of the OpenAI model to use for chat completion</param>
        /// <param name="apikey">API key of OpenAI, if passed empty string, it will try
        /// to get the API key using OPENAI_API_KEY environment variable.</param>
        public OpenAILanguageModel(string model, string apikey = "") 
        {
            this.model = model;
            if(string.IsNullOrEmpty(apikey))
            {
                apikey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            }

            var headers = new Dictionary<string, string>()
            {
                { "Authorization", $"Bearer {apikey}" }
            };

            httpTool = HttpTool.WithClient().WithDefaultRequestHeaders(headers);
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

        /// <summary>
        /// Gets response based on given history of messages.
        /// </summary>
        /// <param name="messages">A list of messages as a history. The Response is generated for  
        /// the last message using the history of messages as context.</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>An LLMResponse response object</returns>
        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
        {
            return await GetResponseAsync(messages, null, temperature);
        }

        /// <summary>
        /// If the language model supports function calling then this method can be called to
        /// get the response based on the given history of messages. 
        /// </summary>
        /// <param name="messages">A list of messages as a history. The response is generated for 
        /// the last message using the history of messages as context.</param>
        /// <param name="functions">A list of function descriptors to match if the request resolves 
        /// to function calling.</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>An LLMResponse response object</returns>
        public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            return GetResponseAsync(messages, functions, temperature).GetAwaiter().GetResult();
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

        /// <summary>
        /// Gets response from the language model asynchronously.
        /// </summary>
        /// <param name="messages">A list of messages as a history. The response is generated for 
        /// the last message using the history of messages as context.</param>
        /// <param name="functions">A list of function descriptors to match if the request resolves 
        /// to function calling.</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>An LLMResponse response object</returns>
        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    ChatRequest data = null;
                    if (functions == null || !functions.Any())
                    {
                        data = new ChatRequest()
                        {
                            model = model,
                            messages = messages.ToArray(),
                            temperature = temperature
                        };
                    }
                    else
                    {
                        var funcs = ToFunctions(functions);
                        data = new FunctionRequest()
                        {
                            model = model,
                            messages = messages.ToArray(),
                            temperature = temperature,
                            functions = funcs
                        };
                    }

                    var serializer = new JavaScriptSerializer();

                    string jsonPayload = serializer.Serialize(data);
                    Logger.WriteLog(LogLevel.Info, LogOps.Request, jsonPayload);

                    string json = await httpTool.PostAsync(apiUrl, jsonPayload);
                    
                    var response = serializer.Deserialize<ChatResponse>(json);
                    var llmResponse = ToLLMResponse(response);
                    Logger.WriteLog(LogLevel.Info, LogOps.Response, llmResponse.Response);
                    return llmResponse;
                }
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
