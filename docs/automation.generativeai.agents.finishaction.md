# FinishAction

Namespace: Automation.GenerativeAI.Agents

Represents the final action that holds the final output or error string if there was an error.

```csharp
public class FinishAction : AgentAction
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AgentAction](./automation.generativeai.agents.agentaction.md) → [FinishAction](./automation.generativeai.agents.finishaction.md)

## Properties

### **Output**

Gets the output

```csharp
public string Output { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Error**

Gets the error string if the agent has met with an error.

```csharp
public string Error { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Parameters**

List of input parameters

```csharp
public IEnumerable<string> Parameters { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### **Tool**

Gets the tool that this action can execute.

```csharp
public IFunctionTool Tool { get; protected set; }
```

#### Property Value

[IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>

### **Thought**

Gets the reasoning for this action.

```csharp
public string Thought { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **ExecutionContext**

Execution context for this action

```csharp
public ExecutionContext ExecutionContext { get; }
```

#### Property Value

[ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>

## Constructors

### **FinishAction(String, String)**

Constructor

```csharp
public FinishAction(string output, string error)
```

#### Parameters

`output` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The final output

`error` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The error string

## Methods

### **ExecuteAsync()**

Executes the given action

```csharp
public Task<string> ExecuteAsync()
```

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Output string
