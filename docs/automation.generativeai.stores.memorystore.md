# MemoryStore

Namespace: Automation.GenerativeAI.Stores

Represents long term memory store

```csharp
public class MemoryStore : Automation.GenerativeAI.Interfaces.IMemoryStore
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [MemoryStore](./automation.generativeai.stores.memorystore.md)<br>
Implements [IMemoryStore](./automation.generativeai.interfaces.imemorystore.md)

## Constructors

### **MemoryStore()**

```csharp
public MemoryStore()
```

## Methods

### **Configure(Int32, IVectorStore)**

Configures the memory store

```csharp
public void Configure(int maxCharacters, IVectorStore vectorStore)
```

#### Parameters

`maxCharacters` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max characters allowed for chat history

`vectorStore` [IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
Vector store to be used for semantic search

### **AddMessage(ChatMessage)**



```csharp
public void AddMessage(ChatMessage message)
```

#### Parameters

`message` [ChatMessage](./automation.generativeai.interfaces.chatmessage.md)<br>

### **ChatHistory(String)**

Gets the chat history

```csharp
public IEnumerable<ChatMessage> ChatHistory(string query)
```

#### Parameters

`query` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;ChatMessage&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **FromJsonFile(String)**

Creates memory store from a Json file

```csharp
public static MemoryStore FromJsonFile(string jsonFile)
```

#### Parameters

`jsonFile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the json file

#### Returns

[MemoryStore](./automation.generativeai.stores.memorystore.md)<br>

### **Save(String)**

Saves the memory to a given file path

```csharp
public void Save(string filepath)
```

#### Parameters

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **Clear()**

Clears the chat history

```csharp
public void Clear()
```
