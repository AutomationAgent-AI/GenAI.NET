# CombineTool

Namespace: Automation.GenerativeAI.Tools

```csharp
public class CombineTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [CombineTool](./automation.generativeai.tools.combinetool.md)<br>
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

## Constructors

### **CombineTool()**

```csharp
public CombineTool()
```

## Methods

### **ExecuteCoreAsync(ExecutionContext)**

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **GetDescriptor()**

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
