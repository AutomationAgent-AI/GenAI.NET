# FunctionTool

Namespace: Automation.GenerativeAI.Tools

The base implementation of IFunctionTool interface as FunctionTool

```csharp
public abstract class FunctionTool : Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md)<br>
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

### **WithName(String)**

Updates the name of the tool

```csharp
public FunctionTool WithName(string name)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to update

#### Returns

[FunctionTool](./automation.generativeai.tools.functiontool.md)<br>
The updated FunctionTool object

### **WithDescription(String)**

Updates description of the tool

```csharp
public FunctionTool WithDescription(string description)
```

#### Parameters

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool to be used for discovery

#### Returns

[FunctionTool](./automation.generativeai.tools.functiontool.md)<br>
The updated FunctionTool object

### **GetDescriptor()**

Returns function descriptor for the tool to make it discoverable by Agent

```csharp
protected abstract FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor

### **ExecuteCoreAsync(ExecutionContext)**

Executes the tool with given context

```csharp
protected abstract Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Result

### **ExecuteAsync(ExecutionContext)**

Executes this tool asynchronously.

```csharp
public Task<string> ExecuteAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
ExecutionContext

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Output string

### **Convert(String, TypeDescriptor)**

Convert the given string data to a give type, either by parsing or deserializing from JSON.

```csharp
public static object Convert(string data, TypeDescriptor type)
```

#### Parameters

`data` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Input data as string

`type` [TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>
Type description for data conversion.

#### Returns

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
Object of the desired type.

### **ToJsonString(Object)**

Utility method to serialize a given object to JSON string

```csharp
public static string ToJsonString(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
Object to serialize

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
JSON string

### **Deserialize&lt;T&gt;(String)**

Utility method to Deserialize the given json string to specific object type.

```csharp
public static T Deserialize<T>(string json)
```

#### Type Parameters

`T`<br>
Object type

#### Parameters

`json` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Input JSON string

#### Returns

T<br>
Deserialized object

### **IsJsonString(String)**

Utility method to check if the input string is a JSON string.

```csharp
public static bool IsJsonString(string str)
```

#### Parameters

`str` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
input string

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if JSON
