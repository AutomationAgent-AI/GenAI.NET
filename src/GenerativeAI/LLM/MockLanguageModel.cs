using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.LLM
{
    /// <summary>
    /// Mock language model can be used for testing.
    /// </summary>
    public class MockLanguageModel : ILanguageModel
    {
        private readonly string model;
        private readonly Dictionary<string, string> responses;
        private readonly OpenAIClient client = new OpenAIClient(new OpenAIConfig());

        /// <summary>
        /// Create the language model instance with a dictionary of request and response.
        /// </summary>
        /// <param name="model">Name of the model.</param>
        /// <param name="responses">Mapping of request and response</param>
        public MockLanguageModel(string model, Dictionary<string, string> responses)
        {
            this.model = model;
            this.responses = new Dictionary<string, string>();
            foreach (var pair in responses)
            {
                var key = Regex.Replace(pair.Key, @"[\W+]", string.Empty);
                this.responses.Add(key.ToLower(), pair.Value);
            }
        }

        /// <summary>
        /// Gets model name
        /// </summary>
        public string ModelName => model;

        public IVectorTransformer VectorTransformer => client.VectorTransformer;

        public int PromptTokensUsed => throw new NotImplementedException();

        public int CompletionTokensUsed => throw new NotImplementedException();

        public int MaxTokenLimit => 4000; //default is 4K

        /// <summary>
        /// Gets the response for given list of chat messages
        /// </summary>
        /// <param name="messages">List of chat messages</param>
        /// <param name="temperature">Not in use.</param>
        /// <returns>An LLMResponse response object</returns>
        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
        {
            return await GetResponseAsync(messages, null, temperature);
        }

        private bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        private double FuzzyMatch(string s1, string s2)
        {
            var substring = TextObject.LongestCommonSubstring(s1, s2);
            double len = Math.Min(s1.Length, s2.Length);
            var match = substring.Length / len;
            return match;
        }

        private string GetBestResponse(string message)
        {
            var txt = Regex.Replace(message, @"[\W+]", string.Empty).ToLower();

            var pair = responses.OrderByDescending(p => FuzzyMatch(p.Key, txt)).First();
            return pair.Value;
        }

        /// <summary>
        /// If the language model supports function calling then this method can be called to
        /// get the response based on the given history of messages. 
        /// </summary>
        /// <param name="messages">A list of messages as a history. The response is generated for 
        /// the last message using the history of messages as context.</param>
        /// <param name="functions">A list of function descriptors to match if the request resolves 
        /// to function calling.</param>
        /// <param name="temperature">Not in use.</param>
        /// <returns>An LLMResponse response object</returns>
        public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            var data = new ChatRequest() { model = model, messages = messages.ToArray(), temperature = temperature };
            var serializer = new JavaScriptSerializer();
            string jsonPayload = serializer.Serialize(data);
            Logger.WriteLog(LogLevel.Info, LogOps.Request, jsonPayload);

            var msg = messages.Last();
            var response = GetBestResponse(msg.content);

            Logger.WriteLog(LogLevel.Info, LogOps.Response, response);

            var responsetype = ResponseType.Done;
            if (functions != null && FunctionTool.IsJsonString(response))
            {
                var responseobj = serializer.DeserializeObject(response);
                if (IsDictionary(responseobj))
                {
                    var dict = responseobj as IDictionary<string, object>;
                    object name;
                    if (dict.TryGetValue("name", out name) && functions.Any(f => f.Name.Contains((string)name)))
                    {
                        responsetype = ResponseType.FunctionCall;
                    }
                }
            }

            return new LLMResponse() { Type = responsetype, Response = response };
        }

        /// <summary>
        /// Gets response from the language model asynchronously.
        /// </summary>
        /// <param name="messages">A list of messages as a history. The response is generated for 
        /// the last message using the history of messages as context.</param>
        /// <param name="functions">A list of function descriptors to match if the request resolves 
        /// to function calling.</param>
        /// <param name="temperature">Not in use.</param>
        /// <returns>An LLMResponse response object</returns>
        public async Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            return await Task.Run(() => GetResponse(messages, functions, temperature));
        }
    }
}
