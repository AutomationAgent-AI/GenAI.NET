# MapReduceTool

Namespace: Automation.GenerativeAI.Tools

Implements a map reduce tool, which can take a mapper tool to map the input collection to
 an intermediate data using mapper tool and then reduce the intermediate data to the final
 output using reducer tool. The mapper runs in parallel, hence mapper tool needs to ensure
 thread safety.

```csharp
public class MapReduceTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [MapReduceTool](./automation.generativeai.tools.mapreducetool.md)<br>
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

### **WithMapperReducer(IFunctionTool, IFunctionTool)**

Creates an instance of MapReduceTool using mapper and reducer tool.

```csharp
public static MapReduceTool WithMapperReducer(IFunctionTool mapper, IFunctionTool reducer)
```

#### Parameters

`mapper` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
A tool to map the input collection to an intermediate data.

`reducer` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
A tool to process the intermediate data to reduce to the final output.

#### Returns

[MapReduceTool](./automation.generativeai.tools.mapreducetool.md)<br>
MapReduceTool

#### Exceptions

[ArgumentNullException](https://docs.microsoft.com/en-us/dotnet/api/system.argumentnullexception)<br>

### **ExecuteCoreAsync(ExecutionContext)**

Implements the core logic to execute the map reduce operations asynchronously

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **GetDescriptor()**

Overrides the GetDescriptor method to return it's FunctionDescriptor. This
 tool has input parameters same as mapper tool but of type array.

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>
FunctionDescriptor
