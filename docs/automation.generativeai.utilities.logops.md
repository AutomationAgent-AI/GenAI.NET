# LogOps

Namespace: Automation.GenerativeAI.Utilities

Different operations type for which log information can be written

```csharp
public enum LogOps
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [ValueType](https://docs.microsoft.com/en-us/dotnet/api/system.valuetype) → [Enum](https://docs.microsoft.com/en-us/dotnet/api/system.enum) → [LogOps](./automation.generativeai.utilities.logops.md)<br>
Implements [IComparable](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable), [IFormattable](https://docs.microsoft.com/en-us/dotnet/api/system.iformattable), [IConvertible](https://docs.microsoft.com/en-us/dotnet/api/system.iconvertible)

## Fields

| Name | Value | Description |
| --- | --: | --- |
| NotFound | 0 | Log when result not found |
| Found | 1 | Log when result is found |
| Classified | 2 | Log when result is classified to a type/class. |
| TextExtracted | 3 | Log when text is extracted |
| Exception | 4 | Log when exception happend |
| Command | 5 | Log while running a command |
| Settings | 6 | Log while updating settings |
| Result | 7 | Log information related to result |
| Request | 8 | Log information related to a request |
| Response | 9 | Log information related to a response |
| Test | 10 | Log information while running a test |
