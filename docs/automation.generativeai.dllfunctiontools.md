# DLLFunctionTools

Namespace: Automation.GenerativeAI

Implements a FunctionToolSet for a .net dll. Using reflection it
 extracts all the public static methods from a given class in the DLL
 to provide it as toolset.

```csharp
public class DLLFunctionTools : Automation.GenerativeAI.Interfaces.IFunctionToolSet, System.Collections.Generic.IEnumerable`1[[Automation.GenerativeAI.Interfaces.IFunctionTool, GenerativeAI, Version=1.1.8661.30687, Culture=neutral, PublicKeyToken=null]], System.Collections.IEnumerable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [DLLFunctionTools](./automation.generativeai.dllfunctiontools.md)<br>
Implements [IFunctionToolSet](./automation.generativeai.interfaces.ifunctiontoolset.md), [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1), [IEnumerable](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ienumerable)

## Properties

### **Item**

```csharp
public IFunctionTool Item { get; }
```

#### Property Value

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>

## Constructors

### **DLLFunctionTools(String, String)**

Creates DLLFunctionTools object

```csharp
public DLLFunctionTools(string dllpath, string classname)
```

#### Parameters

`dllpath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of a .net DLL

`classname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A fully qualified classname, including namespace.

## Methods

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

### **GetFunctions()**

Returns function descriptors for all the public static functions of a class
 this toolset wraps.

```csharp
public IEnumerable<FunctionDescriptor> GetFunctions()
```

#### Returns

[IEnumerable&lt;FunctionDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **GetTool(String)**

Gets a tool for a specific function name.

```csharp
public IFunctionTool GetTool(string functionName)
```

#### Parameters

`functionName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function to search

#### Returns

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
IFunctionTool instance for the corresponding function.

### **GetEnumerator()**

Returns an enumerator that iterates through the collection.

```csharp
public IEnumerator<IFunctionTool> GetEnumerator()
```

#### Returns

[IEnumerator&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerator-1)<br>
An enumerator that can be used to iterate through the collection.
