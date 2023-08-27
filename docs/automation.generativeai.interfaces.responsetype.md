# ResponseType

Namespace: Automation.GenerativeAI.Interfaces

Provides the type of response received from the language model

```csharp
public enum ResponseType
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [ResponseType](./automation.generativeai.interfaces.responsetype.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| Failed | 0 | Request failed |
| Done | 1 | Request is processed successfully |
| Partial | 2 | Request is partially complete |
| FunctionCall | 3 | Request ended with function call details |
