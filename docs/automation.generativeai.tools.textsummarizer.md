# TextSummarizer

Namespace: Automation.GenerativeAI.Tools

```csharp
public class TextSummarizer : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [TextSummarizer](./automation.generativeai.tools.textsummarizer.md)<br>
Implements [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)

## Fields

### **Parameter**

The input text parameter description for the TextSummarizer tool.

```csharp
public static ParameterDescriptor Parameter;
```

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

### **TextSummarizer()**

```csharp
public TextSummarizer()
```

## Methods

### **WithMapReduce(String, String)**

Creates a text summarizer tool with map reduce algorithm

```csharp
public static TextSummarizer WithMapReduce(string mapperPrompt, string reducerPrompt)
```

#### Parameters

`mapperPrompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A prompt to summarize each chunk from the text.

`reducerPrompt` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A prompt to get combined summary.

#### Returns

[TextSummarizer](./automation.generativeai.tools.textsummarizer.md)<br>
TextSummarizer

### **WithLanguageModel(ILanguageModel)**

Sets language model to the agent

```csharp
public TextSummarizer WithLanguageModel(ILanguageModel languageModel)
```

#### Parameters

`languageModel` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
Language model for agent to perform certain tasks

#### Returns

[TextSummarizer](./automation.generativeai.tools.textsummarizer.md)<br>
This TextSummarizer

### **WithTemperature(Double)**

Sets the temperature parameter for the tool to define the creativity.

```csharp
public TextSummarizer WithTemperature(double temperature)
```

#### Parameters

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 and 1 to define creativity

#### Returns

[TextSummarizer](./automation.generativeai.tools.textsummarizer.md)<br>
This TextSummarizer

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
