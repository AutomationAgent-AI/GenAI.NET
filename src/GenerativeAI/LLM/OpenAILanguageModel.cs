using Automation.GenerativeAI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private readonly OpenAIClient openAIClient;

        /// <summary>
        /// Creates OpenAILanguageModel instance.
        /// </summary>
        /// <param name="model">Name of the OpenAI model to use for chat completion</param>
        /// <param name="apikey">API key of OpenAI, if passed empty string, it will try
        /// to get the API key using OPENAI_API_KEY environment variable.</param>
        public OpenAILanguageModel(string model, string apikey = "") 
        {
            if(string.IsNullOrEmpty(apikey))
            {
                apikey = Configuration.Instance.OpenAIConfig.ApiKey;
            }

            openAIClient = new OpenAIClient(new OpenAIConfig() { ApiKey = apikey, Model = model });
        }

        public string ModelName => openAIClient.ModelName;

        public IVectorTransformer VectorTransformer => openAIClient.VectorTransformer;

        public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
        {
            return openAIClient.GetResponseAsync(messages, temperature);
        }

        public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            return openAIClient.GetResponseAsync(messages, functions, temperature);
        }

        public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
        {
            return openAIClient.GetResponseAsync(messages, functions, temperature).GetAwaiter().GetResult();
        }
    }
}
