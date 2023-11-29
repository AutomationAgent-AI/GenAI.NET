# IGenerativeAIService

Namespace: Automation.GenerativeAI.Interfaces

Generative AI Service interface.

```csharp
public interface IGenerativeAIService
```

## Methods

### **CreateOpenAIModel(String, String)**

Creates an instance of a OpenAI Language Model

```csharp
ILanguageModel CreateOpenAIModel(string model, string apikey)
```

#### Parameters

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
OpenAI model name

`apikey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
API Key, if passed empty string then it will try to get the
 the key using environment variable OPENAI_API_KEY.

#### Returns

[ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
OpenAI Language Model instance

### **CreateAzureOpenAIModel(String, String, String, String, String, String)**

Creates an instance of Azure OpenAI Language Model

```csharp
ILanguageModel CreateAzureOpenAIModel(string model, string azureEndpoint, string gptDeployment, string embeddingDeployment, string apiversion, string apiKey)
```

#### Parameters

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Azure OpenAI model name

`azureEndpoint` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Endpoint URL for Azure OpenAI service

`gptDeployment` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Deployment Name for GPT model

`embeddingDeployment` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Deployment Name for text embedding model

`apiversion` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
API version

`apiKey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
ApiKey for the language model

#### Returns

[ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
Azure OpenAI Language Model instance

### **CreateVectorStore(IVectorTransformer)**

Creates a new Vector store with a given transformer. A transformer transforms a text object
 to vector.

```csharp
IVectorStore CreateVectorStore(IVectorTransformer transformer)
```

#### Parameters

`transformer` [IVectorTransformer](./automation.generativeai.interfaces.ivectortransformer.md)<br>
Type of transformer. If transformer is null, it will create use default
 transformer using Bag of words.

#### Returns

[IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
Returns vector store

### **DeserializeVectorStore(String)**

Instantiates Vector Store by deserializing a given vdb file.

```csharp
IVectorStore DeserializeVectorStore(string vdbfile)
```

#### Parameters

`vdbfile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A file with vdb extension for vector store.

#### Returns

[IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
IVectorStore object for a given vdb file

### **CreateVectorTransformer(TransformerType)**

Creates a vector transformer of a given type.

```csharp
IVectorTransformer CreateVectorTransformer(TransformerType type)
```

#### Parameters

`type` [TransformerType](./automation.generativeai.interfaces.transformertype.md)<br>
Type of transformer such as BagOfWords

#### Returns

[IVectorTransformer](./automation.generativeai.interfaces.ivectortransformer.md)<br>

### **RegisterTransformerConstructor(TransformerType, Func&lt;IVectorTransformer&gt;)**

Other services or modules can implement a specific type of transformer and register
 its cunstructor method with this service.

```csharp
void RegisterTransformerConstructor(TransformerType type, Func<IVectorTransformer> constructor)
```

#### Parameters

`type` [TransformerType](./automation.generativeai.interfaces.transformertype.md)<br>
Transformer type

`constructor` [Func&lt;IVectorTransformer&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.func-1)<br>
A constructor to return specific transformer type

### **CreateConversation(String, ILanguageModel)**

Creates a conversation Object with a given context Id.

```csharp
IConversation CreateConversation(string contextId, ILanguageModel languageModel)
```

#### Parameters

`contextId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Unique Id to track the conversation context.

`languageModel` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
Languagel Model to be used for conversation

#### Returns

[IConversation](./automation.generativeai.interfaces.iconversation.md)<br>
New Conversation Object

### **GetConversation(String)**

Retrives existing conversation of specific context.

```csharp
IConversation GetConversation(string contextId)
```

#### Parameters

`contextId` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Conversation context id

#### Returns

[IConversation](./automation.generativeai.interfaces.iconversation.md)<br>
Existing Conversation Object or null
