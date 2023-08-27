# IVectorStore

Namespace: Automation.GenerativeAI.Interfaces

Represents a vector store to store embedding vectors along with its attributes.

```csharp
public interface IVectorStore
```

## Properties

### **VectorLength**

Required length of the vector for the store

```csharp
public abstract int VectorLength { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### **Add(IEnumerable&lt;ITextObject&gt;, Boolean)**

Vectorizes and adds a given textObject to the store.

```csharp
void Add(IEnumerable<ITextObject> textObjects, bool savetext)
```

#### Parameters

`textObjects` [IEnumerable&lt;ITextObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of Input text objects to add to the store

`savetext` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Flag whether to save text as attribute in the store

### **Add(Double[], IDictionary&lt;String, String&gt;)**

Method to add a vector and its attributes to the database.

```csharp
void Add(Double[] vector, IDictionary<string, string> attributes)
```

#### Parameters

`vector` [Double[]](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
Input vector, must be of the same length as VectorLength property of this object

`attributes` [IDictionary&lt;String, String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.idictionary-2)<br>
Attributes for the vector

### **Search(Double[], Int32)**

Method to search the nearest vector objects.

```csharp
IEnumerable<IMatchedObject> Search(Double[] vector, int resultcount)
```

#### Parameters

`vector` [Double[]](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
Input vector

`resultcount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of results expected

#### Returns

[IEnumerable&lt;IMatchedObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of matched objects

### **Search(ITextObject, Int32)**

Method to search objects similar to the input text object

```csharp
IEnumerable<IMatchedObject> Search(ITextObject textObject, int resultcount)
```

#### Parameters

`textObject` [ITextObject](./automation.generativeai.interfaces.itextobject.md)<br>
Input text object

`resultcount` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Number of results expected

#### Returns

[IEnumerable&lt;IMatchedObject&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of matched objects

### **Save(String)**

Saves the vector database to specificed file path.

```csharp
void Save(string filepath)
```

#### Parameters

`filepath` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Vector DB file path
