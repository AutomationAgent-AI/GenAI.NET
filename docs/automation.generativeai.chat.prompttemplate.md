# PromptTemplate

Namespace: Automation.GenerativeAI.Chat

Represents Prompt Template

```csharp
public class PromptTemplate
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [PromptTemplate](./automation.generativeai.chat.prompttemplate.md)

## Properties

### **Template**

Gets the original template string

```csharp
public string Template { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **Variables**

Gets the list of variables in the template

```csharp
public List<string> Variables { get; private set; }
```

#### Property Value

[List&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1)<br>

### **Role**

Reole of the author of the message created by this prompt, the default value is 'user'

```csharp
public Role Role { get; private set; }
```

#### Property Value

[Role](./automation.generativeai.interfaces.role.md)<br>

## Constructors

### **PromptTemplate(String, Role)**

Default constructor that takes template string. The variable input
 must be represented by {{$input}} text in the template, where
 input is the variable value in the template.

```csharp
public PromptTemplate(string template, Role role)
```

#### Parameters

`template` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Template string

`role` [Role](./automation.generativeai.interfaces.role.md)<br>
Role of the author of this prompt, the default value is user. 
 The role could also be system or assistant.

## Methods

### **WithTemplateFile(String, Role)**

Creates the prompt template with a template file.

```csharp
public static PromptTemplate WithTemplateFile(string templatefile, Role role)
```

#### Parameters

`templatefile` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
Full path of the template file.

`role` [Role](./automation.generativeai.interfaces.role.md)<br>
Role of the author of this prompt.

#### Returns

[PromptTemplate](./automation.generativeai.chat.prompttemplate.md)<br>
PromptTemplate

### **FormatMessage(ExecutionContext)**

Formats the template to the ChatMessage using the context dictionary.

```csharp
public ChatMessage FormatMessage(ExecutionContext context)
```

#### Parameters

`context` [ExecutionContext](./automation.generativeai.interfaces.executioncontext.md)<br>
Dictionary containing variables and corresponding values.

#### Returns

[ChatMessage](./automation.generativeai.interfaces.chatmessage.md)<br>
A ChatMessage object
