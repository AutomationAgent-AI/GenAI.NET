# Agent

Namespace: Automation.GenerativeAI.Agents

Represents an Agent that can perform certain actions to accomplish given objective.

```csharp
public abstract class Agent
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [Agent](./automation.generativeai.agents.agent.md)

## Properties

### **Name**

Name of the Agent

```csharp
public string Name { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Description**

Description of the agent

```csharp
public string Description { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### **WithDescription(String)**

Sets description

```csharp
public Agent WithDescription(string description)
```

#### Parameters

`description` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Description of the agent.

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **WithTools(IEnumerable&lt;IFunctionTool&gt;)**

Sets a list of allowed tools for agent to use.

```csharp
public Agent WithTools(IEnumerable<IFunctionTool> tools)
```

#### Parameters

`tools` [IEnumerable&lt;IFunctionTool&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>
List of tools

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **WithLanguageModel(ILanguageModel)**

Sets language model to the agent

```csharp
public Agent WithLanguageModel(ILanguageModel languageModel)
```

#### Parameters

`languageModel` [ILanguageModel](./automation.generativeai.interfaces.ilanguagemodel.md)<br>
Language model for agent to perform certain tasks

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **WithTemperature(Double)**

Sets the temperature parameter to agent to define the creativity.

```csharp
public Agent WithTemperature(double temperature)
```

#### Parameters

`temperature` [Double](https://docs.microsoft.com/en-us/dotnet/api/system.double)<br>
A value between 0 and 1 to define creativity

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **WithMaxAllowedSteps(Int32)**

Sets the maximum number of steps this agent can execute.

```csharp
public Agent WithMaxAllowedSteps(int maxAllowedSteps)
```

#### Parameters

`maxAllowedSteps` [Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>
Maximum number of steps that can be executed.

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **WithMemoryStore(IMemoryStore)**

Sets the memory store for the agent

```csharp
public Agent WithMemoryStore(IMemoryStore memory)
```

#### Parameters

`memory` [IMemoryStore](./automation.generativeai.interfaces.imemorystore.md)<br>
External memory store

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
This Agent

### **GetNextActionAsync()**

Provides a next agent action based on the given message history.

```csharp
protected abstract Task<AgentAction> GetNextActionAsync()
```

#### Returns

[Task&lt;AgentAction&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
AgentAction

### **ExecuteAsync(String)**

Executes the given objective

```csharp
public Task<AgentAction> ExecuteAsync(string objective)
```

#### Parameters

`objective` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The detailed objective for agent to achieve.

#### Returns

[Task&lt;AgentAction&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
FinishAction if objective is met or got an error. It may return AgentAction if the
 tool associated with the action can't be executed. It provides clients to execute the
 action logic and then call UpdateToolResponseAsync to proceed further with execution.

### **UpdateAgentActionResponseAsync(String, String)**

Called by the client to update the tool's execution result with the agent, if the tool
 corresponding to the agent action was executed by client.

```csharp
public Task<AgentAction> UpdateAgentActionResponseAsync(string toolName, string output)
```

#### Parameters

`toolName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the tool that executed

`output` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Output of the tool.

#### Returns

[Task&lt;AgentAction&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
FinishAction if objective is met or got an error. AgentAction if it is
 required to be executed by client.

### **LoadSystemPrompt(String, String, String)**

The derived class implements this method to provide a system prompt text for the language model.

```csharp
protected abstract string LoadSystemPrompt(string username, string date, string workingdir)
```

#### Parameters

`username` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Name of the user interactive with agent

`date` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Today's date

`workingdir` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the working directory

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
System prompt text

### **Create(String, String, AgentType)**

Creates an agent

```csharp
public static Agent Create(string name, string workingdir, AgentType type)
```

#### Parameters

`name` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
A unique name of the agent

`workingdir` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the working directory

`type` [AgentType](./automation.generativeai.agents.agenttype.md)<br>
Type of the agent to create

#### Returns

[Agent](./automation.generativeai.agents.agent.md)<br>
Agent

### **MessgeFromResponse(LLMResponse)**

Converts the LLMResponse to ChatMessage

```csharp
protected internal static ChatMessage MessgeFromResponse(LLMResponse response)
```

#### Parameters

`response` [LLMResponse](./automation.generativeai.interfaces.llmresponse.md)<br>
A response returned from the language model.

#### Returns

[ChatMessage](./automation.generativeai.interfaces.chatmessage.md)<br>
ChatMessage
