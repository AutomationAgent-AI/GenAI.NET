# SearchTool

Namespace: Automation.GenerativeAI.Tools

SearchTool implementation

```csharp
public abstract class SearchTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [SearchTool](./automation.generativeai.tools.searchtool.md)<br>
Implements [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)

## Fields

### **QueryParameter**

The query parameter description for the search tool.

```csharp
public static ParameterDescriptor QueryParameter;
```

### **ContextParameter**

The context parameter for the search tool.

```csharp
public static ParameterDescriptor ContextParameter;
```

## Properties

### **Name**

Name of the tool

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Description**

Description of the tool

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Descriptor**

Gets the function descriptor used for tool discovery by agents and LLM

```csharp
public FunctionDescriptor Descriptor { get; }
```

#### Property Value

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>

## Methods

### **SearchAsync(String, String)**

Performs search

```csharp
public abstract Task<IEnumerable<SearchResult>> SearchAsync(string query, string context)
```

#### Parameters

`query` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
What to search

`context` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Optional parameter, Context from where to search

#### Returns

[Task&lt;IEnumerable&lt;SearchResult&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Search results

### **ForBingSearch(String)**

Creates BingSearch tool with parameters

```csharp
public static SearchTool ForBingSearch(string apiKey)
```

#### Parameters

`apiKey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
API key

#### Returns

[SearchTool](./automation.generativeai.tools.searchtool.md)<br>
SearchTool for Bing Search

### **ForSemanticeSearch(IVectorStore)**

Creates a search tool using a vector store for semantic search.

```csharp
public static SearchTool ForSemanticeSearch(IVectorStore store)
```

#### Parameters

`store` [IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
Vector store to be used for semantic search

#### Returns

[SearchTool](./automation.generativeai.tools.searchtool.md)<br>
SearchTool for semantic search

### **ForSemanticSearchFromDatabase(String)**

Creates a search tool for semantic search using a given vector database

```csharp
public static SearchTool ForSemanticSearchFromDatabase(string dbPath)
```

#### Parameters

`dbPath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the vector database

#### Returns

[SearchTool](./automation.generativeai.tools.searchtool.md)<br>
SearchTool

#### Exceptions

[FileNotFoundException](https://docs.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception)<br>

### **ForSemanticSearchFromSource(String, Int32, Int32, TransformerType)**

Creates a search tool for semantic search using a given source, such as 
 Document or Text content. If source is a document, then it needs full path
 for the document including wildcard path. This tool uses a pipeline of TextExtractor,
 TextSplitter and Semantic Search tools.

```csharp
public static SearchTool ForSemanticSearchFromSource(string source, int chunkSize, int chunkOverlap, TransformerType transformerType)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Plain text or full path of document to be used for semantic search.

`chunkSize` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max number of characters in the chunk to be used by text splitter

`chunkOverlap` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max number of character overlap between the two consecutive chunks.

`transformerType` [TransformerType](./automation.generativeai.interfaces.transformertype.md)<br>
Type of transformer to be used for indexing

#### Returns

[SearchTool](./automation.generativeai.tools.searchtool.md)<br>
SearchTool for semantic search

### **WithMaxResultCount(Int32)**

Sets max result count parameter to the tool. The default value is 5.

```csharp
public SearchTool WithMaxResultCount(int count)
```

#### Parameters

`count` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Count of maximum results to be expected by this search tool.

#### Returns

[SearchTool](./automation.generativeai.tools.searchtool.md)<br>
SearchTool

### **ExecuteCoreAsync(ExecutionContext)**

Overrides the core implementation logic to execute the search tool.

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
ExecutionContext

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Execution Result

### **GetDescriptor()**

Provides function descriptor for this search tool.

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor
