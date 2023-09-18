# ObjectTypeDescriptor

Namespace: Automation.GenerativeAI

Describes an Object Type

```csharp
public class ObjectTypeDescriptor : TypeDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TypeDescriptor](./automation.generativeai.typedescriptor.md) → [ObjectTypeDescriptor](./automation.generativeai.objecttypedescriptor.md)

## Properties

### **Properties**

List of properties/parameters of the object type

```csharp
public List<ParameterDescriptor> Properties { get; set; }
```

#### Property Value

[List&lt;ParameterDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Type**

Type of the parameter, possible values are string, number etc.

```csharp
public string Type { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ObjectTypeDescriptor(IEnumerable&lt;ParameterDescriptor&gt;)**

Constructor

```csharp
public ObjectTypeDescriptor(IEnumerable<ParameterDescriptor> properties)
```

#### Parameters

`properties` [IEnumerable&lt;ParameterDescriptor&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

## Methods

### **UpdateProperties(Dictionary&lt;String, Object&gt;)**

Updates the properties of the object type

```csharp
protected void UpdateProperties(Dictionary<string, object> properties)
```

#### Parameters

`properties` [Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
