# Pipeline

Namespace: Automation.GenerativeAI.Tools

Represents a simple pipeline of tools, where output of previous tool is input of next tool.

```csharp
public class Pipeline : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [Pipeline](./automation.generativeai.tools.pipeline.md)<br>
Implements [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)

## Properties

### **Tools**

Gets a list of tools successfully added to the pipeline.

```csharp
public IEnumerable<IFunctionTool> Tools { get; }
```

#### Property Value

[IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

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

### **WithTools(IEnumerable&lt;IFunctionTool&gt;)**

A method to create Pipeline with a list of tools. If any of the tool in the list can't be added 
 successfully, then that tool will be skipped from the pipeline.

```csharp
public static Pipeline WithTools(IEnumerable<IFunctionTool> tools)
```

#### Parameters

`tools` [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of tools that needs to be executed sequntially.

#### Returns

[Pipeline](./automation.generativeai.tools.pipeline.md)<br>
Pipeline

### **TryAddTool(IFunctionTool)**

Tries to add a new tool to this pipeline. If the added tool is a follow
 up tool then it must have only one parameter. The pipeline can't resolve
 more than one parameters for execution.

```csharp
public bool TryAddTool(IFunctionTool tool)
```

#### Parameters

`tool` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
A function tool to add

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if the tool is added successfully.

### **ExecuteCoreAsync(ExecutionContext)**

Executes the tool with given context

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Execution Result

### **GetDescriptor()**

Returns function descriptor for the tool to make it discoverable by Agent

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor
