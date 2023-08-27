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

Gets AI assitant's response for a given message.

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

### **GetFiles(String, String[])**

```csharp
internal static IEnumerable<string> GetFiles(string source, String[] extensions)
```

#### Parameters

`source` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`extensions` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **SaveForSemanticSearch(String, String, Int32, Int32)**

Splits the texts from the given source into smaller chunks based on chunkSize parameter and 
 serializes them to a vector database for semantic search.

```csharp
public static IVectorStore SaveForSemanticSearch(string source, string destination, int chunkSize, int overlap)
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
