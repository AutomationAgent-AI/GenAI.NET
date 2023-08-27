# MockLanguageModel

Namespace: Automation.GenerativeAI.LLM

Mock language model can be used for testing.

```csharp
public class MockLanguageModel : Automation.GenerativeAI.Interfaces.ILanguageModel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MockLanguageModel](./automation.generativeai.llm.mocklanguagemodel.md)<br>
Implements [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)

## Properties

### **ModelName**

Gets model name

```csharp
public string ModelName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **MockLanguageModel(String, Dictionary&lt;String, String&gt;)**

Create the language model instance with a dictionary of request and response.

```csharp
public MockLanguageModel(string model, Dictionary<string, string> responses)
```

#### Parameters

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the model.

`responses` [Dictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
Mapping of request and response

## Methods

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, Double)**

Gets the response for given list of chat messages

```csharp
public Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of chat messages

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 2, that controls randomness of the response. 
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
A value between 0 to 2, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[LLMResponse](./automation.generativeai.interfaces.llmresponse.md)<br>
An LLMResponse response object

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
A value between 0 to 2, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
An LLMResponse response object
