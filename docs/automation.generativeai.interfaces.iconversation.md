# IConversation

Namespace: Automation.GenerativeAI.Interfaces

Represents a conversation chain.

```csharp
public interface IConversation
```

## Properties

### **Id**

ID of the conversation, passed during creation of this object.

```csharp
public abstract string Id { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **AppendMessage(String, Role)**

Append message to the conversation.

```csharp
void AppendMessage(string message, Role role)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Message text

`role` [Role](./automation.generativeai.interfaces.role.md)<br>
Role of the messanger

### **AppendMessage(ChatMessage)**

Append message to the conversation.

```csharp
void AppendMessage(ChatMessage message)
```

#### Parameters

`message` [ChatMessage](./automation.generativeai.interfaces.chatmessage.md)<br>
Message text

### **AddContext(IEnumerable&lt;ITextObject&gt;)**

Adds context to the conversation by providing external data source.

```csharp
void AddContext(IEnumerable<ITextObject> context)
```

#### Parameters

`context` [IEnumerable&lt;ITextObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of Text Objects as external data source

### **AddContext(IVectorStore)**

Adds context to the conversation by providing external data source.

```csharp
void AddContext(IVectorStore contextStore)
```

#### Parameters

`contextStore` [IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
Vector store as context

### **AddToolSet(IFunctionToolSet)**

Adds a toolset to the conversation, so that it can execute appropriate
 function from the toolset to generate response.

```csharp
void AddToolSet(IFunctionToolSet toolSet)
```

#### Parameters

`toolSet` [IFunctionToolSet](./automation.generativeai.interfaces.ifunctiontoolset.md)<br>
An IFunctionToolSet object.

### **GetResponseAsync(Double)**

Gets the response from AI on the current conversation asynchronously.

```csharp
Task<ChatMessage> GetResponseAsync(double temperature)
```

#### Parameters

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 2, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
A ChatMessage or FunctionCallMessage object.
