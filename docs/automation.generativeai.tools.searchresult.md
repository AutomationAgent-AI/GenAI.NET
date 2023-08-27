# SearchResult

Namespace: Automation.GenerativeAI.Tools

SearchResult as returned by the SearchTool

```csharp
public class SearchResult
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [SearchResult](./automation.generativeai.tools.searchresult.md)

## Properties

### **content**

Content snippet

```csharp
public string content { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **reference**

Reference URL or document name

```csharp
public string reference { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **SearchResult()**

```csharp
public SearchResult()
```

## Methods

### **Equals(Object)**

Override Equals method to just compare references.

```csharp
public bool Equals(object obj)
```

#### Parameters

`obj` [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object)<br>
Other object to be compared with.

#### Returns

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
True if both objects are same.

### **GetHashCode()**

Overrides the GetHashCode method to return hash code based on reference.

```csharp
public int GetHashCode()
```

#### Returns

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
