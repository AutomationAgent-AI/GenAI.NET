# ExecutionContext

Namespace: Automation.GenerativeAI.Interfaces

Represents the execution context that holds all the parameters required for the execution.

```csharp
public class ExecutionContext
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)

## Properties

### **Item**

```csharp
public object Item { get; set; }
```

#### Property Value

[Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>

## Constructors

### **ExecutionContext()**

Default constructor

```csharp
public ExecutionContext()
```

### **ExecutionContext(Dictionary&lt;String, Object&gt;)**

Creates ExecutionContext with given parameters

```csharp
public ExecutionContext(Dictionary<string, object> parameters)
```

#### Parameters

`parameters` [Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
Parameters dictionary

## Methods

### **GetParameters()**

```csharp
internal IDictionary<string, object> GetParameters()
```

#### Returns

[IDictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br>

### **TryGetResult(String, Object&)**

Tries to get the result for specific tool

```csharp
public bool TryGetResult(string toolname, Object& result)
```

#### Parameters

`toolname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool

`result` [Object&](https://docs.microsoft.com/en-us/dotnet/api/system.object&)<br>
Execution result if available

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if successful

### **AddResult(String, Object)**

Adds the result of the given tool to the context

```csharp
public void AddResult(string toolname, object result)
```

#### Parameters

`toolname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool.

`result` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
Execution Result of the tool

### **TryGetValue(String, Object&)**

```csharp
internal bool TryGetValue(string variable, Object& value)
```

#### Parameters

`variable` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`value` [Object&](https://docs.microsoft.com/en-us/dotnet/api/system.object&)<br>

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
