# TextObject

Namespace: Automation.GenerativeAI.Interfaces

Represents a text object

```csharp
public class TextObject : ITextObject
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TextObject](./automation.generativeai.interfaces.textobject.md)<br>
Implements [ITextObject](./automation.generativeai.interfaces.itextobject.md)

## Fields

### **DefaultClass**

Default class name for the text object classification

```csharp
public static string DefaultClass;
```

## Properties

### **Name**

Name or source of the text object, something like document name

```csharp
public string Name { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Class**

The classification of the text object

```csharp
public string Class { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Text**

The text content

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

Creates Text Object

```csharp
public static ITextObject Create(string name, string text, string classname)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the object, something like document name

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The text content

`classname` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The classification of the text object

#### Returns

[ITextObject](./automation.generativeai.interfaces.itextobject.md)<br>
ITextObject

### **FuzzyMatch(String, String)**

Compares two string to provide match score using fuzzy logic

```csharp
public static double FuzzyMatch(string s1, string s2)
```

#### Parameters

`s1` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
First string

`s2` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Second string

#### Returns

[Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
Match score between 0 and 1

### **LongestCommonSubstring(String, String)**

Gets the longest common substring between the two given strings

```csharp
public static string LongestCommonSubstring(string s1, string s2)
```

#### Parameters

`s1` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
First string

`s2` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Second string

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Longest common string
