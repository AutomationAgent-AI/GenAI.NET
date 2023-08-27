# LLMResponse

Namespace: Automation.GenerativeAI.Interfaces

Response returned by the large language model for a give message

```csharp
public class LLMResponse
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [LLMResponse](./automation.generativeai.interfaces.llmresponse.md)

## Properties

### **Response**

Response returned by the Language Model

```csharp
public string Response { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Type**

Response type returned by the language model

```csharp
public ResponseType Type { get; set; }
```

#### Property Value

[ResponseType](./automation.generativeai.interfaces.responsetype.md)<br>

## Constructors

### **LLMResponse()**

Default constructor

```csharp
public LLMResponse()
```
