# TypeDescriptor

Namespace: Automation.GenerativeAI

Describes a type

```csharp
public class TypeDescriptor
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TypeDescriptor](./automation.generativeai.typedescriptor.md)

## Properties

### **Type**

Type of the parameter, possible values are string, number etc.

```csharp
public string Type { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **StringType**

StringType

```csharp
public static TypeDescriptor StringType { get; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

### **NumberType**

NumberType

```csharp
public static TypeDescriptor NumberType { get; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

### **IntegerType**

IntegerType

```csharp
public static TypeDescriptor IntegerType { get; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

### **BooleanType**

BooleanType

```csharp
public static TypeDescriptor BooleanType { get; }
```

#### Property Value

[TypeDescriptor](./automation.generativeai.typedescriptor.md)<br>

## Constructors

### **TypeDescriptor(String)**

Default constructor

```csharp
public TypeDescriptor(string type)
```

#### Parameters

`type` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Type name

## Methods

### **ToDictionary()**

Converts the type to a dictionary for JSON serialization

```csharp
public Dictionary<string, object> ToDictionary()
```

#### Returns

[Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

### **UpdateProperties(Dictionary&lt;String, Object&gt;)**

Updates properties of type descriptor

```csharp
protected void UpdateProperties(Dictionary<string, object> properties)
```

#### Parameters

`properties` [Dictionary&lt;String, Object&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>
