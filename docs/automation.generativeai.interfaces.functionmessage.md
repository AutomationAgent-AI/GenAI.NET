# FunctionMessage

Namespace: Automation.GenerativeAI.Interfaces

Represents a function message to send the function call output to the 
 language model to summarize the result in natural language.

```csharp
public class FunctionMessage : ChatMessage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatMessage](./automation.generativeai.interfaces.chatmessage.md) → [FunctionMessage](./automation.generativeai.interfaces.functionmessage.md)

## Properties

### **name**

Name of the function that was called

```csharp
public string name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **role**

Role of the messenger

```csharp
public string role { get; }
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

### **FunctionMessage(String, String)**

Create a function message

```csharp
public FunctionMessage(string name, string output)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function that was executed

`output` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Output values returned by the function
