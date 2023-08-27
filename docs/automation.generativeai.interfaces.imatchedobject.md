# IMatchedObject

Namespace: Automation.GenerativeAI.Interfaces

Vector match object

```csharp
public interface IMatchedObject
```

## Properties

### **Score**

Matching score

```csharp
public abstract double Score { get; }
```

#### Property Value

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>

### **Attributes**

Attributes of the matched object

```csharp
public abstract IDictionary<string, string> Attributes { get; }
```

#### Property Value

[IDictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br>
