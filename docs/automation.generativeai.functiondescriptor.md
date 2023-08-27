# FunctionDescriptor

Namespace: Automation.GenerativeAI

Provides description of a function

```csharp
public class FunctionDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [FunctionDescriptor](./automation.generativeai.functiondescriptor.md)

## Properties

### **Name**

Name of the function

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Description**

Description of the function

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Parameters**

Parameters details as needed by the function

```csharp
public ObjectTypeDescriptor Parameters { get; set; }
```

#### Property Value

[ObjectTypeDescriptor](./automation.generativeai.objecttypedescriptor.md)<br>

### **InputParameters**

Provides a list of input parameters.

```csharp
public IEnumerable<string> InputParameters { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Constructors

### **FunctionDescriptor(String, String, IEnumerable&lt;ParameterDescriptor&gt;)**

Default constructor

```csharp
public FunctionDescriptor(string name, string description, IEnumerable<ParameterDescriptor> parameters)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the fucntion

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the function

`parameters` [IEnumerable&lt;ParameterDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of parameters
