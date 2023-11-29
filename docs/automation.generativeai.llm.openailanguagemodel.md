# OpenAILanguageModel

Namespace: Automation.GenerativeAI.LLM

OpenAI language model implementation

```csharp
public class OpenAILanguageModel : Automation.GenerativeAI.Interfaces.ILanguageModel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [OpenAILanguageModel](./automation.generativeai.llm.openailanguagemodel.md)<br>
Implements [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)

## Properties

### **ModelName**

```csharp
public string ModelName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **VectorTransformer**

```csharp
public IVectorTransformer VectorTransformer { get; }
```

#### Property Value

[IVectorTransformer](./automation.generativeai.interfaces.ivectortransformer.md)<br>

## Constructors

### **OpenAILanguageModel(String, String)**

Creates OpenAILanguageModel instance.

```csharp
public OpenAILanguageModel(string model, string apikey)
```

#### Parameters

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the OpenAI model to use for chat completion

`apikey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
API key of OpenAI, if passed empty string, it will try
 to get the API key using OPENAI_API_KEY environment variable.

## Methods

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, Double)**

```csharp
public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, IEnumerable&lt;FunctionDescriptor&gt;, Double)**

```csharp
public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **GetResponse(IEnumerable&lt;ChatMessage&gt;, IEnumerable&lt;FunctionDescriptor&gt;, Double)**

```csharp
public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[LLMResponse](./automation.generativeai.interfaces.llmresponse.md)<br>
