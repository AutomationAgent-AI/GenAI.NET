# Application

Namespace: Automation.GenerativeAI

Generative AI application class that instantiates GenerativeAIService.

```csharp
public class Application
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Application](./automation.generativeai.application.md)

## Properties

### **DefaultLanguageModel**

Provides an instance of the default OpenAI language model

```csharp
public static ILanguageModel DefaultLanguageModel { get; }
```

#### Property Value

[ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>

## Constructors

### **Application()**

Default constructor

```csharp
public Application()
```

## Methods

### **SetLanguageModel(ILanguageModel)**

```csharp
internal static void SetLanguageModel(ILanguageModel llm)
```

#### Parameters

`llm` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>

### **GetAIService()**

Returns the generative AI service interface

```csharp
public static IGenerativeAIService GetAIService()
```

#### Returns

[IGenerativeAIService](./automation.generativeai.interfaces.igenerativeaiservice.md)<br>

### **GetTextProviderService()**

```csharp
internal static TextProviderService GetTextProviderService()
```

#### Returns

[TextProviderService](./automation.generativeai.services.textproviderservice.md)<br>

### **Initialize(String, String, String, String)**

Application must be initialized before making any request.

```csharp
public void Initialize(string llmtype, string model, string apikey, string logFilePath)
```

#### Parameters

`llmtype` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Large Language Model type such as OpenAI

`model` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Model name

`apikey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
apikey for the language model

`logFilePath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path for log files.

#### Exceptions

[NotImplementedException](https://docs.microsoft.com/en-us/dotnet/api/system.notimplementedexception)<br>

### **SetLogFilePath(String)**

Sets Log file path

```csharp
public static void SetLogFilePath(string logFilePath)
```

#### Parameters

`logFilePath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the log file

### **GetResponseAsync(String, Double)**

Gets AI assitant's response for a given message. This request doesn't have any memory.

```csharp
public static Task<string> GetResponseAsync(string message, double temperature)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
User's message

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 to 2, that controls randomness of the response. 
 Higher temperature will lead to more randomness. Lower temperature will be more deterministic.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Response returned by the AI assitant.

### **GetResponse(String, Double)**

Sends message to language model to get AI assistant's response.

```csharp
public static string GetResponse(string message, double temperature)
```

#### Parameters

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
User's message

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Response message

### **GetFiles(String, String[])**

```csharp
internal static IEnumerable<string> GetFiles(string source, String[] extensions)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`extensions` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **CreateVectorDatabaseForSemanticSearch(String, String, Int32, Int32)**

Splits the texts from the given source into smaller chunks based on chunkSize parameter and 
 serializes them to a vector database for semantic search.

```csharp
public static IVectorStore CreateVectorDatabaseForSemanticSearch(string source, string destination, int chunkSize, int overlap)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Source of information, it could be full path of a folder 
 or file. If the full path of a folder is provided, it will collect all the 
 supported files from the folder.

`destination` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path where the vector database needs to be saved.

`chunkSize` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max number of characters of the chunk to be used by text splitter.

`overlap` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max number of character overlap between the two consecutive chunks.

#### Returns

[IVectorStore](./automation.generativeai.interfaces.ivectorstore.md)<br>
The vector store where data is saved

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **GetResponseFromContextAsync(String, String, String, String, Double)**

Gets response from AI assitant using the context vector database.

```csharp
public static Task<string> GetResponseFromContextAsync(string sessionid, string systemctx, string vdbpath, string message, double temperature)
```

#### Parameters

`sessionid` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Session id or the conversation id

`systemctx` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
System message to specify, how to process a given message.

`vdbpath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of vector database to get the context.

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
User message for which response is required.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 and 2 to control the randomness of the response.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Returns the AI assitant's response.

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **GetResponseFromContext(String, String, String, String, Double)**

Gets response from AI assitant using the context vector database.

```csharp
public static string GetResponseFromContext(string sessionid, string systemctx, string vdbpath, string message, double temperature)
```

#### Parameters

`sessionid` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Session id or the conversation id

`systemctx` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
System message to specify, how to process a given message.

`vdbpath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of vector database to get the context.

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
User message for which response is required.

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 and 2 to control the randomness of the response.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Returns the AI assitant's response.

#### Exceptions

[Exception](https://docs.microsoft.com/en-us/dotnet/api/system.exception)<br>

### **CreateFunctionDescription(String, String, String)**

Creates a function description that can be passed to language model

```csharp
public static int CreateFunctionDescription(string name, string description, string parametersTableCSVAsText)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the function

`parametersTableCSVAsText` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A table to contain parameter name, description and type

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Returns function id

### **AddToolsFromDLL(String)**

Adds all the public static methods exposed from DLL as function tool

```csharp
public static List<string> AddToolsFromDLL(string dllpath)
```

#### Parameters

`dllpath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the DLL

#### Returns

[List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
List of the tools name that got added to the tools collection.

### **AddFunctionMessage(String, String, String, Double)**

Adds a function message as a response to function_call message.

```csharp
public static string AddFunctionMessage(string sessionid, string functionName, string message, double temperature)
```

#### Parameters

`sessionid` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The session id for the conversation.

`functionName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the function that was executed.

`message` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Output returned from the function call as string(json).

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 and 2 to control the randomness of the response.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The response from the language model.

### **ExecuteTool(String, String)**

Executes a given tool with the arguments passed and returns the 
 output as a string. For complex objects it returns JSON string.

```csharp
public static string ExecuteTool(string name, string executionContext)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to execute

`executionContext` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A JSON string with key value pairs as a context holding variable values to execute the tool.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Output string as a result of the execution if successful, else error message starting with ERROR.

### **GetExecutionResult(String)**

Gets the execution result for a given tool. Useful in getting intermediate results.

```csharp
public static string GetExecutionResult(string toolname)
```

#### Parameters

`toolname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Execution result if available, else error statement.

### **GetToolInputParameters(String)**

Returns a list of input parameters for the given tool.

```csharp
public static List<string> GetToolInputParameters(string toolName)
```

#### Parameters

`toolName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool

#### Returns

[List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
List of input parameters

### **GetToolInputParameterType(String, String)**

Returns the type of a given input parameter.

```csharp
public static string GetToolInputParameterType(string toolName, string parameter)
```

#### Parameters

`toolName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool

`parameter` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the parameter

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Type of parameter such as string, number, integer, boolean, array etc.

### **RemoveTool(String)**

Removes the given tool from the toolset.

```csharp
public static string RemoveTool(string toolName)
```

#### Parameters

`toolName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation

### **CreatePromptTool(String, String, String)**

Creates a prompt tool and adds it to tools collection.

```csharp
public static string CreatePromptTool(string name, string description, string template)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`template` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Template string with input variables. The variable input
 must be represented by {{$input}} text in the template, where
 input is the variable value in the template.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateQueryTool(String, String, String)**

Creates a query tool and adds it to tools collection. The query tool makes use of
 registered language model to get response of the query.

```csharp
public static string CreateQueryTool(string name, string description, string template)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`template` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Template string with input variables. The variable input
 must be represented by {{$input}} text in the template, where
 input is the variable value in the template.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateToolsPipeline(String, String, List&lt;String&gt;)**

Creates a pipeline with a list of tools. A pipeline executes tools sequentially, where
 output of previous tools can be input of the next tool in the sequence.

```csharp
public static string CreateToolsPipeline(string name, string description, List<string> tools)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`tools` [List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>
List of tools name to be added to the pipeline.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateMapReduceTool(String, String, String, String)**

Creates MapReduce tool which can take a mapper tool to map the input collection to
 an intermediate data using mapper tool and then reduce the intermediate data to the final
 output using reducer tool. The mapper runs in parallel, hence mapper tool needs to ensure
 thread safety.

```csharp
public static string CreateMapReduceTool(string name, string description, string mapper, string reducer)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`mapper` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to map the input collection to an intermediate data.

`reducer` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool to process the intermediate data to reduce to the final output.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateCombineTool(String, String)**

Creates a combine tool to combine an array of string using newline.

```csharp
public static string CreateCombineTool(string name, string description)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateBingSearchTool(String, String, String, Int32)**

Creates a bing search tool to search specific query.

```csharp
public static string CreateBingSearchTool(string name, string description, string apikey, int maxResultsCount)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`apikey` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
API key for Bing Search

`maxResultsCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Count of maximum results to be expected by this search tool.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateSemanticSearchTool(String, String, String, Int32)**

Creates a search tool for semantic search using a given vector database

```csharp
public static string CreateSemanticSearchTool(string name, string description, string dbpath, int maxResultsCount)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`dbpath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the vector database. The vector database can be created using 
 CreateVectorDatabaseForSemanticSearch method.

`maxResultsCount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Count of maximum results to be expected by this search tool.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation.

### **CreateHttpRequestTool(String, String, String)**

Creates HttpTool that can be used to make GET/POST/PUT/DELETE requests to any uri.

```csharp
public static string CreateHttpRequestTool(string name, string description, string requestHeaderJson)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`requestHeaderJson` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The key value pairs for header information in a JSON format.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation

### **CreateTextExtractorTool(String, String)**

Creates a text extractor tool that extracts text from different files. Currently
 txt, csv and pdf files are supported.

```csharp
public static string CreateTextExtractorTool(string name, string description)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation

### **CreateDataExtractorTool(String, String, String)**

Creates a tool that can extract data from an unstructured text using language model.

```csharp
public static string CreateDataExtractorTool(string name, string description, string parametersJson)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool, it must be unique.

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the tool.

`parametersJson` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The key value pairs for name and description for parameters to be extracted
 in a JSON format.

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Status of the operation
