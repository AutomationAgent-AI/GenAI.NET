# TextSplitter

Namespace: Automation.GenerativeAI.Tools

Splits the given text into smaller chunks

```csharp
public class TextSplitter
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [TextSplitter](./automation.generativeai.tools.textsplitter.md)

## Methods

### **WithParameters(Int32, Int32)**

Creates a text splitter with parameters

```csharp
public static TextSplitter WithParameters(int chunkSize, int chunkOverlap)
```

#### Parameters

`chunkSize` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Max character length for each chunk

`chunkOverlap` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Size of chunk overlap

#### Returns

[TextSplitter](./automation.generativeai.tools.textsplitter.md)<br>
TextSplitter object

### **SplitOnTokens(String)**

Splits the given text based on tokens/separators.

```csharp
protected IEnumerable<string> SplitOnTokens(string text)
```

#### Parameters

`text` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Input text

#### Returns

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of tokens

### **Split(ITextObject)**

Splits the given text object into smaller chunks.

```csharp
public IEnumerable<ITextObject> Split(ITextObject text)
```

#### Parameters

`text` [ITextObject](./automation.generativeai.interfaces.itextobject.md)<br>
TextObject

#### Returns

[IEnumerable&lt;ITextObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of splitted text objects
