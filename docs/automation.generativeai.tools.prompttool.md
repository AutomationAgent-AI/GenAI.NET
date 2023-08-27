# PromptTool

Namespace: Automation.GenerativeAI.Tools

A simple tool to evaluate prommpt.

```csharp
public class PromptTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [PromptTool](./automation.generativeai.tools.prompttool.md)<br>
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

### **WithTemplate(String)**

Creates a new instance of PrompTool with template string

```csharp
public static PromptTool WithTemplate(string prompttemplate)
```

#### Parameters

`prompttemplate` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Prompt template string.

#### Returns

[PromptTool](./automation.generativeai.tools.prompttool.md)<br>
A new PromptTool

### **Execute(ExecutionContext, String&)**

Executes this tool with the given context

```csharp
protected bool Execute(ExecutionContext context, String& output)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context containing template variable values.

`output` [String&](https://docs.microsoft.com/en-us/dotnet/api/system.string&)<br>
Output string

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful

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
Result

### **GetDescriptor()**

Returns a descriptor object for this tool

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor
