# TextExtractorTool

Namespace: Automation.GenerativeAI.Tools

TextExtractTool that extracts text from a given one or more source text file or pdf from a directory.

```csharp
public class TextExtractorTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [TextExtractorTool](./automation.generativeai.tools.textextractortool.md)<br>
Implements [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)

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

### **Create()**

Creates a TextExtractorTool

```csharp
public static TextExtractorTool Create()
```

#### Returns

[TextExtractorTool](./automation.generativeai.tools.textextractortool.md)<br>
TextExtractorTool

### **ExtractTextObjects(String)**

```csharp
internal static List<ITextObject> ExtractTextObjects(string source)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[List&lt;ITextObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **ExtractText(String)**

Extracts text from a given source. The source can be a full path of a file
 or a folder containing txt or pdf files.

```csharp
public static string ExtractText(string source)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path to get a list of files.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Text content from all the files from source.

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **Execute(ExecutionContext, String&)**

Executes the text extractor tool to extract text based on the execution context

```csharp
protected bool Execute(ExecutionContext context, String& output)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
ExecutionContext

`output` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
Extracted text content

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful

### **GetDescriptor()**

Provides function descriptor

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor

### **ExecuteCoreAsync(ExecutionContext)**

Overrides the core execution logic

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
ExecutionContext

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Execution Result
