# ToolsCollection

Namespace: Automation.GenerativeAI.Tools

Represents a simple collection of tools as a toolset

```csharp
public class ToolsCollection : Automation.GenerativeAI.Interfaces.IFunctionToolSet, System.Collections.Generic.IEnumerable`1[[Automation.GenerativeAI.Interfaces.IFunctionTool, GenerativeAI, Version=1.0.8640.40506, Culture=neutral, PublicKeyToken=null]], System.Collections.IEnumerable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ToolsCollection](./automation.generativeai.tools.toolscollection.md)<br>
Implements [IFunctionToolSet](./automation.generativeai.interfaces.ifunctiontoolset.md), [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public IFunctionTool Item { get; }
```

#### Property Value

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>

## Constructors

### **ToolsCollection(IFunctionTool)**

Constructs the ToolsCollection with a single tool.

```csharp
public ToolsCollection(IFunctionTool tool)
```

#### Parameters

`tool` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
IFunctionTool to be added to the toolset/collection.

### **ToolsCollection(IEnumerable&lt;IFunctionTool&gt;)**

Constructs the ToolsCollection with a list of tools.

```csharp
public ToolsCollection(IEnumerable<IFunctionTool> tools)
```

#### Parameters

`tools` [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of IFunctionTool to be added to the collection.

## Methods

### **AddTool(IFunctionTool)**

Adds a given tool to the collection.

```csharp
public void AddTool(IFunctionTool tool)
```

#### Parameters

`tool` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
IFunctionTool to be added to the toolset/collection.

### **AddTools(IEnumerable&lt;IFunctionTool&gt;)**

Adds a given list of tools to the collection.

```csharp
public void AddTools(IEnumerable<IFunctionTool> tools)
```

#### Parameters

`tools` [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of IFunctionTool to be added to the collection.

### **RemoveTool(String)**

Removes a tool with given name from the collection.

```csharp
public bool RemoveTool(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to be removed from the collection.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if a tool is successfully found in the collection and removed.

### **ExecuteAsync(String, ExecutionContext)**

Executes a given function with the arguments passed and returns the 
 output as a string. For complex objects it returns JSON string.

```csharp
public Task<string> ExecuteAsync(string functionName, ExecutionContext context)
```

#### Parameters

`functionName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function to execute

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context holding arguments of the function

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Output string as a result of the execution

### **GetEnumerator()**

Returns an enumerator that iterates through the collection.

```csharp
public IEnumerator<IFunctionTool> GetEnumerator()
```

#### Returns

[IEnumerator&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br>
An enumerator that can be used to iterate through the collection.

### **GetFunctions()**

Returns function descriptors for all the public static functions of a class
 this toolset wraps.

```csharp
public IEnumerable<FunctionDescriptor> GetFunctions()
```

#### Returns

[IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
A list of function descriptors

### **GetTool(String)**

Gets a tool for a specific name.

```csharp
public IFunctionTool GetTool(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to search

#### Returns

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
IFunctionTool instance if present in the collection.
