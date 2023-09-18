# ArrayTypeDescriptor

Namespace: Automation.GenerativeAI

Represents an Array type

```csharp
public class ArrayTypeDescriptor : TypeDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TypeDescriptor](./automation.generativeai.typedescriptor.md) → [ArrayTypeDescriptor](./automation.generativeai.arraytypedescriptor.md)

## Properties

### **ItemType**

If the type is array, it represents the type of item.

```csharp
public TypeDescriptor ItemType { get; set; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

### **Type**

Type of the parameter, possible values are string, number etc.

```csharp
public string Type { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **ArrayTypeDescriptor(TypeDescriptor)**

Default contructor

```csharp
public ArrayTypeDescriptor(TypeDescriptor itemType)
```

#### Parameters

`itemType` [TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

## Methods

### **UpdateProperties(Dictionary&lt;String, Object&gt;)**

Updates the properties of Array type

```csharp
protected void UpdateProperties(Dictionary<string, object> properties)
```

#### Parameters

`properties` [Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
