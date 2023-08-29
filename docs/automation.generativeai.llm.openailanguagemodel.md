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

Name of the model used within OpenAI

```csharp
public string ModelName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

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

Gets response based on given history of messages.

```csharp
public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of messages as a history. The Response is generated for 
 the last message using the history of messages as context.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 1, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
An LLMResponse response object

### **GetResponse(IEnumerable&lt;ChatMessage&gt;, IEnumerable&lt;FunctionDescriptor&gt;, Double)**

If the language model supports function calling then this method can be called to
 get the response based on the given history of messages.

```csharp
public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of messages as a history. The response is generated for 
 the last message using the history of messages as context.

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of function descriptors to match if the request resolves 
 to function calling.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 1, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[LLMResponse](./automation.generativeai.interfaces.llmresponse.md)<br>
An LLMResponse response object

### **ToFunctions(IEnumerable&lt;FunctionDescriptor&gt;)**

```csharp
internal static List<Dictionary<string, object>> ToFunctions(IEnumerable<FunctionDescriptor> functions)
```

#### Parameters

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[List&lt;Dictionary&lt;String, Object&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **ToJSON(IEnumerable&lt;FunctionDescriptor&gt;)**

```csharp
internal static string ToJSON(IEnumerable<FunctionDescriptor> functions)
```

#### Parameters

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, IEnumerable&lt;FunctionDescriptor&gt;, Double)**

Gets response from the language model asynchronously.

```csharp
public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of messages as a history. The response is generated for 
 the last message using the history of messages as context.

`functions` [IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of function descriptors to match if the request resolves 
 to function calling.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 1, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
An LLMResponse response object
