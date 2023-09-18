# AgentAction

Namespace: Automation.GenerativeAI.Agents

Represents agent action that can be executed as current step. It holdes a tool
 and execution context so that the tool can be executed.

```csharp
public class AgentAction
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [AgentAction](./automation.generativeai.agents.agentaction.md)

## Properties

### **Parameters**

List of input parameters required for the tool/action.

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
public ExecutionContext ExecutionContext { get; private set; }
```

#### Property Value

[ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>

## Constructors

### **AgentAction(IFunctionTool, ExecutionContext, String)**

Constructor

```csharp
public AgentAction(IFunctionTool tool, ExecutionContext executionContext, string thought)
```

#### Parameters

`tool` [IFunctionTool](./automation.generativeai.interfaces.ifunctiontool.md)<br>
The tool this action can execute.

`executionContext` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Execution context to execute the tool.

`thought` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The reasoning of the thought for this action.

## Methods

### **ExecuteAsync()**

Executes the given tool asynchronously.

```csharp
public Task<string> ExecuteAsync()
```

#### Returns

[Task&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
Output string returned from the tool after execution.
