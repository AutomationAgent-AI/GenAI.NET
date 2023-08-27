# Logger

Namespace: Automation.GenerativeAI

```csharp
public class Logger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Logger](./automation.generativeai.logger.md)

## Constructors

### **Logger()**

```csharp
public Logger()
```

## Methods

### **GetLogFile(String)**

```csharp
public static string GetLogFile(string operation)
```

#### Parameters

`operation` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SetLogFile(String, LogLevel)**

```csharp
public static void SetLogFile(string logfile, LogLevel logLevel)
```

#### Parameters

`logfile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

`logLevel` [LogLevel](./automation.generativeai.loglevel.md)<br>

### **WriteLog(LogLevel, LogOps, String)**

```csharp
public static void WriteLog(LogLevel level, LogOps ops, string log)
```

#### Parameters

`level` [LogLevel](./automation.generativeai.loglevel.md)<br>

`ops` [LogOps](./automation.generativeai.logops.md)<br>

`log` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
