# HttpTool

Namespace: Automation.GenerativeAI.Tools

A tool for HTTP requests

```csharp
public class HttpTool : FunctionTool, Automation.GenerativeAI.Interfaces.IFunctionTool, System.IDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionTool](./automation.generativeai.tools.functiontool.md) → [HttpTool](./automation.generativeai.tools.httptool.md)<br>
Implements [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md), [IDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.idisposable)

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

### **WithClient(HttpClient)**

Creates the HttpTool with given client

```csharp
public static HttpTool WithClient(HttpClient httpClient)
```

#### Parameters

`httpClient` HttpClient<br>
HttpClient to send requests

#### Returns

[HttpTool](./automation.generativeai.tools.httptool.md)<br>
HttpTool

### **WithDefaultRequestHeaders(Dictionary&lt;String, String&gt;)**

Updates the tool with default request headers.

```csharp
public HttpTool WithDefaultRequestHeaders(Dictionary<string, string> headers)
```

#### Parameters

`headers` [Dictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
Headers dictionary

#### Returns

[HttpTool](./automation.generativeai.tools.httptool.md)<br>
Updated HttpTool

### **ExecuteCoreAsync(ExecutionContext)**

Implements the execution logic

```csharp
protected Task<Result> ExecuteCoreAsync(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>

#### Returns

[Task&lt;Result&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>

### **GetDescriptor()**

Provides the descriptor of the tool

```csharp
protected FunctionDescriptor GetDescriptor()
```

#### Returns

[FunctionDescriptor](./automation.generativeai.functiondescriptor.md)<br>

### **GetAsync(String, CancellationToken)**

Sends an HTTP GET request to the specified URI and returns the response body as a string.

```csharp
public Task<string> GetAsync(string uri, CancellationToken cancellationToken)
```

#### Parameters

`uri` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
URI of the request

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to use to request cancellation.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
The response body as a string.

### **PostAsync(String, String, CancellationToken)**

Sends an HTTP POST request to the specified URI and returns the response body as a string.

```csharp
public Task<string> PostAsync(string uri, string body, CancellationToken cancellationToken)
```

#### Parameters

`uri` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
URI of the request

`body` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The body of the request

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to use to request cancellation.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
The response body as a string.

### **PutAsync(String, String, CancellationToken)**

Sends an HTTP PUT request to the specified URI and returns the response body as a string.

```csharp
public Task<string> PutAsync(string uri, string body, CancellationToken cancellationToken)
```

#### Parameters

`uri` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
URI of the request

`body` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The body of the request

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to use to request cancellation.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
The response body as a string.

### **DeleteAsync(String, CancellationToken)**

Sends an HTTP DELETE request to the specified URI and returns the response body as a string.

```csharp
public Task<string> DeleteAsync(string uri, CancellationToken cancellationToken)
```

#### Parameters

`uri` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
URI of the request

`cancellationToken` [CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>
The token to use to request cancellation.

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
The response body as a string.

### **Dispose(Boolean)**

Disposes/releases the unmanaged resources.

```csharp
protected void Dispose(bool disposing)
```

#### Parameters

`disposing` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **Dispose()**

Disposes the unmanaged resources and suppresses GC finalization

```csharp
public void Dispose()
```
