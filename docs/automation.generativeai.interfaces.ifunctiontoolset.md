# IFunctionToolSet

Namespace: Automation.GenerativeAI.Interfaces

Represents a function toolset that provides multiple functions

```csharp
public interface IFunctionToolSet : System.Collections.Generic.IEnumerable`1[[Automation.GenerativeAI.Interfaces.IFunctionTool, GenerativeAI, Version=1.1.8641.34377, Culture=neutral, PublicKeyToken=null]], System.Collections.IEnumerable
```

Implements [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public abstract IFunctionTool Item { get; }
```

#### Property Value

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>

## Methods

### **GetFunctions()**

Returns a list of functions supported by the tool

```csharp
IEnumerable<FunctionDescriptor> GetFunctions()
```

#### Returns

[IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of function descriptors

### **ExecuteAsync(String, ExecutionContext)**

Executes a given function with its arguments

```csharp
Task<string> ExecuteAsync(string functionName, ExecutionContext context)
```

#### Parameters

`functionName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Function name to execute

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context object containing all relevant arguments for execution.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Output as string, this can be a JSON string for complex object.

### **GetTool(String)**

Searches a function with the given name in this toolset and returns the 
 corresponding tool.

```csharp
IFunctionTool GetTool(string functionName)
```

#### Parameters

`functionName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function to wrap as tool.

#### Returns

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
IFunctionTool corresponding to the given name.
