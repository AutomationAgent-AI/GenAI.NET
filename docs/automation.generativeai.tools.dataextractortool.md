# DataExtractorTool

Namespace: Automation.GenerativeAI.Tools

Extracts data from a given text based on the schema passed as json string.

```csharp
public class DataExtractorTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [DataExtractorTool](./automation.generativeai.tools.dataextractortool.md)<br>
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

### **DataExtractorTool()**

```csharp
public DataExtractorTool()
```

## Methods

### **Create()**

Creates basic DataExtractorTool

```csharp
public static DataExtractorTool Create()
```

#### Returns

[DataExtractorTool](./automation.generativeai.tools.dataextractortool.md)<br>

### **WithLanguageModel(ILanguageModel)**

Sets language model to the tool

```csharp
public DataExtractorTool WithLanguageModel(ILanguageModel model)
```

#### Parameters

`model` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
The language model implementation

#### Returns

[DataExtractorTool](./automation.generativeai.tools.dataextractortool.md)<br>
Updted DataExtractorTool

### **WithParameters(Dictionary&lt;String, String&gt;)**

Updates the parameters to extract

```csharp
public DataExtractorTool WithParameters(Dictionary<string, string> parameters)
```

#### Parameters

`parameters` [Dictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
Dictionary of parameter name and Description.

#### Returns

[DataExtractorTool](./automation.generativeai.tools.dataextractortool.md)<br>
Updated DataExtractorTool

### **WithJSON(String)**

Updates the parameters to extract using json file or json string.

```csharp
public DataExtractorTool WithJSON(string json)
```

#### Parameters

`json` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
json string or json file containing a list of Name and Description of parameters.

#### Returns

[DataExtractorTool](./automation.generativeai.tools.dataextractortool.md)<br>
Updated DataExtractorTool

### **ExtractDataAsync(String)**

Extracts data based on the parameters provided from the given text asynchronously

```csharp
public Task<Dictionary<string, string>> ExtractDataAsync(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Input text

#### Returns

[Task&lt;Dictionary&lt;String, String&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
A dictionary of parameter name and corresponding values.

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
