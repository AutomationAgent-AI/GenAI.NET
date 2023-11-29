using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Stores;
using System;
using System.Collections.Generic;

namespace Automation.GenerativeAI.Services
{
    class GenerativeAIService : IGenerativeAIService
    {
        private readonly Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();
        private readonly Dictionary<TransformerType, Func<IVectorTransformer>> transformerFactory = new Dictionary<TransformerType, Func<IVectorTransformer>>();

        public GenerativeAIService() 
        { 
            transformerFactory.Add(TransformerType.OpenAIEmbedding, ()=>new OpenAIEmbeddingTransformer());
        }

        public IConversation CreateConversation(string contextId, ILanguageModel languageModel)
        {
            var chat = new Conversation(contextId, languageModel);
            conversations.Add(contextId, chat);
            return chat;
        }

        public IConversation GetConversation(string contextId)
        {
            Conversation conversation;
            if(conversations.TryGetValue(contextId, out conversation)) return conversation;

            return null;
        }

        public IVectorStore DeserializeVectorStore(string vdbfile)
        {
            return VectorStore.Create(vdbfile);
        }

        public IVectorStore CreateVectorStore(IVectorTransformer transformer)
        {
            return new VectorStore(transformer);
        }

        public IVectorTransformer CreateVectorTransformer(TransformerType type)
        {
            Func<IVectorTransformer> constructor;
            if (transformerFactory.TryGetValue(type, out constructor))
                return constructor();

            throw new NotImplementedException();
        }

        public void RegisterTransformerConstructor(TransformerType type, Func<IVectorTransformer> constructor)
        {
            transformerFactory.Add(type, constructor);
        }

        public ILanguageModel CreateOpenAIModel(string model, string apikey)
        {
            if (!string.IsNullOrEmpty(apikey))
            {
                Configuration.Instance.OpenAIConfig = new OpenAIConfig() { ApiKey = apikey, Model = model };
            }
            return new OpenAILanguageModel(model, apikey);
        }

        public ILanguageModel CreateAzureOpenAIModel(string model, string azureEndpoint, string gptDeployment, string embeddingDeployment, string apiversion, string apikey)
        {
            if (!string.IsNullOrEmpty(apikey))
            {
                Configuration.Instance.OpenAIConfig = new OpenAIConfig() { EndPointUrl = azureEndpoint, GPTDeployment = gptDeployment, EmbeddingDeployment = embeddingDeployment, ApiVersion = apiversion, ApiKey = apikey, Model = model };
            }
            return new AzureOpenAILanguageModel(Configuration.Instance.OpenAIConfig);
        }
    }
}
