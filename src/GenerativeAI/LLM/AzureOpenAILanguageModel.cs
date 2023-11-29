using Automation.GenerativeAI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.LLM
{
    internal class AzureOpenAILanguageModel : ILanguageModel
    {
        private readonly OpenAIClient openAIClient;

        /// <summary>
        /// Creates OpenAILanguageModel instance.
        /// </summary>
        /// <param name="model">Name of the OpenAI model to use for chat completion</param>
        /// <param name="apikey">API key of OpenAI, if passed empty string, it will try
        /// to get the API key using OPENAI_API_KEY environment variable.</param>
        public AzureOpenAILanguageModel(OpenAIConfig config)
        {
            openAIClient = new OpenAIClient(config);
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
    }
}
