# IFunctionTool

Namespace: Automation.GenerativeAI.Interfaces

Represents a tool that can be executed with right parameters passed through the context.

```csharp
public interface IFunctionTool
```

## Properties

### **Name**

Gets Name of the tool

```csharp
public abstract string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Descriptor**

Gets Description of the tool including parameters needed to execute.

```csharp
public abstract FunctionDescriptor Descriptor { get; }
```

#### Property Value

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>

## Methods

### **ExecuteAsync(ExecutionContext)**

Executes this tool with the given execution context asynchronously

```csharp
Task<string> ExecuteAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context that holds parameter values required to execute this tool.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Outcome of the execution, for a complex object it will return a json string.
