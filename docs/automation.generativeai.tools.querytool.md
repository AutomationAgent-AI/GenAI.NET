# QueryTool

Namespace: Automation.GenerativeAI.Tools

A Query tool that allows user to use a prompt template to query LLM

```csharp
public class QueryTool : PromptTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [PromptTool](./automation.generativeai.tools.prompttool.md) → [QueryTool](./automation.generativeai.tools.querytool.md)<br>
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

### **WithPromptTemplate(PromptTemplate)**

Creates QueryTool object with a prompt template object.

```csharp
public static QueryTool WithPromptTemplate(PromptTemplate prompt)
```

#### Parameters

`prompt` [PromptTemplate](./automation.generativeai.chat.prompttemplate.md)<br>
Prompt template for the tool

#### Returns

[QueryTool](./automation.generativeai.tools.querytool.md)<br>
QueryTool

### **WithPromptTemplate(String)**

Creates QueryTool object with a prompt template string.

```csharp
public static QueryTool WithPromptTemplate(string template)
```

#### Parameters

`template` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Prompt template for the tool

#### Returns

[QueryTool](./automation.generativeai.tools.querytool.md)<br>
QueryTool

### **WithLanguageModel(ILanguageModel)**

Sets language model to the tool

```csharp
public QueryTool WithLanguageModel(ILanguageModel model)
```

#### Parameters

`model` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
The language model implementation

#### Returns

[QueryTool](./automation.generativeai.tools.querytool.md)<br>
QueryTool

### **ExecuteCoreAsync(ExecutionContext)**

Overrides the core executoion logic to execute this query tool with the given context

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context wtih prompt parameters

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Result
