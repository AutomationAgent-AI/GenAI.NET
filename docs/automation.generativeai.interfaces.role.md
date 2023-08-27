# Role

Namespace: Automation.GenerativeAI.Interfaces

Represents various roles involved in the conversation.

```csharp
public enum Role
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [Role](./automation.generativeai.interfaces.role.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| user | 0 | User usually asks questions in the conversation. |
| assistant | 1 | Assistant is AI BOT that responds to the questions of user. |
| system | 2 | System user provides instruction to BOT on how to respond to certain questions. |
| function | 3 | A function user provides the response returned by a function call  identified by the language model. |
