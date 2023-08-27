# ILanguageModel

Namespace: Automation.GenerativeAI.Interfaces

Defines an implementation of a large language model

```csharp
public interface ILanguageModel
```

## Properties

### **ModelName**

Name of the model

```csharp
public abstract string ModelName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, Double)**

Gets response based on given history of messages.

```csharp
Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of messages as a history. The Response is generated for 
 the last message using the history of messages as context.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 2, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;LLMResponse&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
An LLMResponse response object

### **GetResponseAsync(IEnumerable&lt;ChatMessage&gt;, IEnumerable&lt;FunctionDescriptor&gt;, Double)**

If the language model supports function calling then this method can be called to
 get the response asynchronously based on the given history of messages.

```csharp
Task<LLMResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, IEnumerable<FunctionDescriptor> functions, double temperature)
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
