# TextObject

Namespace: Automation.GenerativeAI

```csharp
public class TextObject : Automation.GenerativeAI.Interfaces.ITextObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TextObject](./automation.generativeai.textobject.md)<br>
Implements [ITextObject](./automation.generativeai.interfaces.itextobject.md)

## Fields

### **DefaultClass**

```csharp
public static string DefaultClass;
```

## Properties

### **Name**

```csharp
public string Name { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Class**

```csharp
public string Class { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Text**

```csharp
public string Text { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Constructors

### **TextObject()**

```csharp
public TextObject()
```

## Methods

### **Create(String, String, String)**

```csharp
public static ITextObject Create(string name, string text, string classname)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`classname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[ITextObject](./automation.generativeai.interfaces.itextobject.md)<br>

### **LongestCommonSubstring(String, String)**

```csharp
public static string LongestCommonSubstring(string s1, string s2)
```

#### Parameters

`s1` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`s2` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **FuzzyMatch(String, String)**

```csharp
public static double FuzzyMatch(string s1, string s2)
```

#### Parameters

`s1` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`s2` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
