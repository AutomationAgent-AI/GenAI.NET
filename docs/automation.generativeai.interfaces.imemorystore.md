# IMemoryStore

Namespace: Automation.GenerativeAI.Interfaces

Represents long term memory store for the language model

```csharp
public interface IMemoryStore
```

## Methods

### **Configure(Int32, IVectorStore)**

Configures the memory store

```csharp
void Configure(int maxCharacters, IVectorStore vectorStore)
```

#### Parameters

`maxCharacters` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max characters allowed for chat history

`vectorStore` [IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
Vector store to be used for semantic search

### **ChatHistory(String)**

Gets the current chat history

```csharp
IEnumerable<ChatMessage> ChatHistory(string query)
```

#### Parameters

`query` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Current query to get relevant history.

#### Returns

[IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **AddMessage(ChatMessage)**

Adds a ChatMessage to the memory

```csharp
void AddMessage(ChatMessage message)
```

#### Parameters

`message` [ChatMessage](./automation.generativeai.interfaces.chatmessage.md)<br>

### **Clear()**

Clears the memory

```csharp
void Clear()
```

### **Save(String)**

Saves the memory to a given file path

```csharp
void Save(string filepath)
```

#### Parameters

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
