# ParameterDescriptor

Namespace: Automation.GenerativeAI

Provides description of a function parameter.

```csharp
public class ParameterDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ParameterDescriptor](./automation.generativeai.parameterdescriptor.md)

## Properties

### **Name**

Name of the parameter

```csharp
public string Name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Description**

Description of the parameter

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Type**

Type of the parameter, possible values are string, number etc.

```csharp
public TypeDescriptor Type { get; set; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

### **Required**

Flag to check if the parameter is required or optional

```csharp
public bool Required { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Constructors

### **ParameterDescriptor()**

```csharp
public ParameterDescriptor()
```
