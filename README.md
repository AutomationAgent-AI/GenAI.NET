# GenAI.NET
A .NET library to interact with Large Language Models such as OpenAI. This library provides the following features.
- Language Model: A default language model to connect to OpenAI and get a response.
- Function Calling: The language mode implementation allows users to pass a list of function descriptions to the language model for extraction of functions and parameters from the request.
- GenerativeAI Service: A convenient service interface to provide some high-level functions.
- Conversation: An interface to interact with the language model while retaining the conversation chain in memory. It also can refer to additional context using vector stores to get a meaningful response based on some documents.
- Function Toolset: An interface that can provide supported function descriptions and execute them. This function tool can be passed to the conversation so that related functions from the tool can be executed if needed.
- DLLFunctionToolset: Wraps all the public static methods of a .NET class from a given DLL into a function toolset. This module refers to the documentation of the static methods to create function descriptions.
- PromptTemplate: Helps you to parameterize your prompts with a template string. You can define a template string with the template variables denoted as {{ $input }}
- Pipeline: Helps to create a pipeline of tools to execute them sequentially.

## API Documentation
[Documentation](https://github.com/automaze1/GenAI.NET/blob/main/docs/index.md)

## Examples
### Basic Conversation Using Language Model

```csharp
//Create OpenAI Language model
var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable
var llm = new OpenAILanguageModel("gpt-3.5-turbo-0613", apikey);

//Create Chat Message with a specific role
var message = new ChatMessage(Role.user, "Hi, there!! I am Ram");

//Get a Response from the language model using a list of chat messages.
var response = await llm.GetResponseAsync(Enumerable.Repeat(message, 1), 0.8);
//response.Response provides the response message
Console.WriteLine(response.Response);
```

### Create a simple conversation to interact with the OpenAI language model.

```csharp
//Create GenAI service
var service = Application.GetAIService();

var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable

//Create Language Model
var llm = service.CreateOpenAIModel("gpt-3.5-turbo-0613", apikey);

//Create Conversation
var chat = service.CreateConversation("Test", llm);
chat.AppendMessage("Hi, there!! I am Ram", Role.user);
var response = await chat.GetResponseAsync(0.8);

Console.WriteLine($"{response.role}: {response.content}");
```

### Function calling

```csharp
//Define function descriptor
var p1 = new ParameterDescriptor()
{
    Name = "location",
    Description = "The city and state, e.g. San Francisco, CA",
};
var p2 = new ParameterDescriptor()
{
    Name = "unit",
    Description = string.Empty,
    Required = false,
    Type = new EnumTypeDescriptor(new string[] { "celsius", "fahrenheit" }),
};
var parameters = new List<ParameterDescriptor>() { p1, p2 };

var name = "get_current_weather";
var description = "Get the current weather in a given location";

var function = new FunctionDescriptor(name, description, parameters);

//Create OpenAI Language model
var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable
var llm = new OpenAILanguageModel("gpt-3.5-turbo-0613", apikey);

//Create user message
var message = new ChatMessage(Role.user, "What is the weather like in Boston?");

//Get a response from the LLM, it must return a function call message.
var response = await llm.GetResponseAsync(
                Enumerable.Repeat(message, 1),
                Enumerable.Repeat(function, 1),
                0.8);
//response.Type == Response.Type.FunctionCall
//response.Response is a JSON object to provide function details
Console.WriteLine($"Response Type: {response.Type}");
Console.WriteLine($"Response Message: {response.Response}");
```
### PromptTemplate and QueryTool

```csharp
//Define a template string, the template variables can be denoted as {{ $input }}
//Following template has two variable adjective and content.
var template = @"Tell me a {{ $adjective }} joke about {{ $content }}.";

//Create PromptTemplate with the template string
var prompt = new PromptTemplate(template, Role.user);

//Define execution context with the parameter values
var context = new ExecutionContext();
context["adjective"] = "funny";
context["content"] = "chicken";

//You can format the message to update the variables in the template string to get the message
var msg = prompt.FormatMessage(context); //msg.content == "Tell me a funny joke about chicken."

Console.WriteLine($"{msg.role}: {msg.content}");

//Create OpenAI Language model
var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable
var llm = new OpenAILanguageModel("gpt-3.5-turbo-0613", apikey);

//Create a QueryTool to process this template with the language model
var tool = QueryTool.WithPromptTemplate(prompt)
                    .WithLanguageModel(llm);

var result = await tool.ExecuteAsync(context); //Returns a funny joke about chicken

Console.WriteLine($"Joke: {result}");
```
### Tools Pipeline
You can create a pipeline of tools to execute them sequentially. Note that the first tool can take in more than one variable however, the subsequent tools must have only one variable, and the input value to this tool will be the result obtained from the previous tool in the pipeline. 

```csharp
//Create OpenAI Language model
var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable
var llm = new OpenAILanguageModel("gpt-3.5-turbo-0613", apikey);

//Define a couple of template strings
var template1 = "Provide me an engaging title for a blog on topic '{{$topic}}' for '{{$audience}}'.";
var template2 = "Give me outline of a blog with title: {{$title}}.";

//Create QueryTool with these prompt templates
var tools = new[]
{
    QueryTool.WithPromptTemplate(template1).WithLanguageModel(llm).WithName("Tool_1"),
    QueryTool.WithPromptTemplate(template2).WithLanguageModel(llm).WithName("Tool_2"),
};

//create pipeline with tools
var pipeline = Pipeline.WithTools(tools);

//Define execution context with the parameter values
var context = new ExecutionContext();
context["topic"] = "Generative AI";
context["audience"] = "Children";

var outline = await pipeline.ExecuteAsync(context); //returns the complete outline for the Blog

//Get the title of the blog from the context
object title;
context.TryGetResult(tools.First().Name, out title);

Console.WriteLine($"TITLE: {title}");
Console.WriteLine();
Console.WriteLine(outline);
```
