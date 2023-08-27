# OpenAILanguageModel

Namespace: Automation.GenerativeAI

```csharp
public class OpenAILanguageModel : Automation.GenerativeAI.Interfaces.ILanguageModel
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [OpenAILanguageModel](./automation.generativeai.openailanguagemodel.md)<br>
Implements [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)

## Properties

### **ModelName**

```csharp
public string ModelName { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **OpenAILanguageModel(String, String)**

```csharp
public OpenAILanguageModel(string model, string apikey)
```

#### Parameters

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`apikey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **GetResponse(IEnumerable&lt;ChatMessage&gt;, Double)**

```csharp
public LLMResponse GetResponse(IEnumerable<ChatMessage> messages, double temperature)
```

#### Parameters

`messages` [IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[LLMResponse](./automation.generativeai.interfaces.llmresponse.md)<br>

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
