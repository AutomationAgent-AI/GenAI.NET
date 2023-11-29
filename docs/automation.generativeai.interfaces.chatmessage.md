# ChatMessage

Namespace: Automation.GenerativeAI.Interfaces

Represents a chat message

```csharp
public class ChatMessage
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ChatMessage](./automation.generativeai.interfaces.chatmessage.md)

## Properties

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

### **ChatMessage(Role, String)**

Creates ChatMessage object with given Role and content

```csharp
public ChatMessage(Role role, string content)
```

#### Parameters

`role` [Role](./automation.generativeai.interfaces.role.md)<br>

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ChatMessage()**

Default Constructor

```csharp
public ChatMessage()
```
