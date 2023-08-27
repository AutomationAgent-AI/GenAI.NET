# Logger

Namespace: Automation.GenerativeAI.Utilities

A simple logger class that helps write log information

```csharp
public class Logger
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Logger](./automation.generativeai.utilities.logger.md)

## Constructors

### **Logger()**

```csharp
public Logger()
```

## Methods

### **GetLogFile(String)**

Gets the log file for specific operation

```csharp
public static string GetLogFile(string operation)
```

#### Parameters

`operation` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **SetLogFile(String, LogLevel)**

Sets the log file and the log level

```csharp
public static void SetLogFile(string logfile, LogLevel logLevel)
```

#### Parameters

`logfile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the log file

`logLevel` [LogLevel](./automation.generativeai.utilities.loglevel.md)<br>
Log information level

### **WriteLog(LogLevel, LogOps, String)**

Write the log information

```csharp
public static void WriteLog(LogLevel level, LogOps ops, string log)
```

#### Parameters

`level` [LogLevel](./automation.generativeai.utilities.loglevel.md)<br>
Log level

`ops` [LogOps](./automation.generativeai.utilities.logops.md)<br>
Log operation

`log` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Log information
