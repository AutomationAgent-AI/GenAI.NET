# EnumTypeDescriptor

Namespace: Automation.GenerativeAI

```csharp
public class EnumTypeDescriptor : TypeDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TypeDescriptor](./automation.generativeai.typedescriptor.md) → [EnumTypeDescriptor](./automation.generativeai.enumtypedescriptor.md)

## Properties

### **Options**

List of possible values for the parameter if applicable, else it could be null.

```csharp
public String[] Options { get; set; }
```

#### Property Value

[String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Type**

Type of the parameter, possible values are string, number etc.

```csharp
public string Type { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **EnumTypeDescriptor(String[])**

```csharp
public EnumTypeDescriptor(String[] options)
```

#### Parameters

`options` [String[]](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **UpdateProperties(Dictionary&lt;String, Object&gt;)**

```csharp
protected void UpdateProperties(Dictionary<string, object> properties)
```

#### Parameters

`properties` [Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
