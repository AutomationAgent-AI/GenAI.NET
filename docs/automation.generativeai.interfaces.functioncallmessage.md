# FunctionCallMessage

Namespace: Automation.GenerativeAI.Interfaces

Represents a function call message usually returned by the language model.

```csharp
public class FunctionCallMessage : ChatMessage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatMessage](./automation.generativeai.interfaces.chatmessage.md) → [FunctionCallMessage](./automation.generativeai.interfaces.functioncallmessage.md)

## Properties

### **function_call**

Function call details that contains function name and arguments of 
 the function as returned by language model. For example, 
 { "name": "get_current_weather",
 "arguments": "{ \"location\": \"Boston, MA\"}" }

```csharp
public Dictionary<string, object> function_call { get; set; }
```

#### Property Value

[Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **role**

Role of the messenger

```csharp
public string role { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **content**

Message content

```csharp
public string content { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **FunctionCallMessage()**

Creates a FunctionCallMessage

```csharp
public FunctionCallMessage()
```
