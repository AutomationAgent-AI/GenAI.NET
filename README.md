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
- MapReduceTool: Helps to process large data set with a mapper tool in parallel and then reduce to the final output.
- SearchTool: Helps to do a web search using Bing API

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
### MapReduce Tool
You can use the MapReduce tool to map a large array of input data using a mapper tool and finally reduce them to final output using a reducer tool. The mapper runs in parallel, hence it is important to use the mapper tool that is threadsafe.

```csharp
//Prompt tool that takes 3 input variables to create a sentence.
var prompt = PromptTool.WithTemplate("The capital of {{$state}} is {{$city}} and '{{$language}}' is the most popular language there.");

//Combines a given array of text by joining the text with a new line.
var combine = new CombineTool();
//Create a MapReduce tool
var mapreduce = MapReduceTool.WithMapperReducer(prompt, combine);

var context = new ExecutionContext();

//The context has an array of data for each input variable from the mapper tool.
context["state"] = new[] { "UP", "Bihar", "Jharkhand", "MP" };
context["city"] = new[] { "Lucknow", "Patna", "Ranchi", "Bhopal" };
context["language"] = new[] { "Hindi", "Bhojpuri", "Santhal", "Hindi" };

var result = await mapreduce.ExecuteAsync(context);

Console.WriteLine(result);
Console.ReadLine();
```

### Search Tool

```csharp
//Get Bing API key
string apiKey = "";

//Create Bing Search tool with max result count 5
var tool = SearchTool.ForBingSearch(apiKey).WithMaxResultCount(5);

var context = new ExecutionContext();
context[tool.Descriptor.InputParameters.First()] = "What is the latest update on India's moon mission?";

//Execute the search query
var result = await tool.ExecuteAsync(context);

Console.WriteLine(result);
```

#### Output:

```JSON
[{"content":"India\u0027s Chandrayaan-3 spacecraft is looking to become the first ever to land on the moon\u0027s south pole, just days after a Russian attempt ended in a crash. The region\u0027s shadowed craters are believed to contain water ice that could help make a permanent lunar base for humans a reality. Wednesday 23 August 2023 11:25, UK India LIVE","reference":"https://news.sky.com/story/india-moon-landing-live-updates-chandrayaan-3-making-historic-bid-to-reach-south-pole-of-lunar-surface-12945600"},{"content":"Chandrayaan-3, India's latest mission to the moon, is set to undertake its key final stage today as the unmanned spacecraft attempts a soft landing on the lunar surface - 40 days after its...","reference":"https://techcrunch.com/2023/08/22/chandrayaan-3-landing-moon-live/"},{"content":"Chandrayaan-3 Moon Landing Successful: India has created history as it became the first country to land on the South Pole of lunar surface. PM Modi congratulated Indians and space scientists...","reference":"https://www.livemint.com/science/news/chandrayaan3-live-updates-chandrayaan-landing-time-india-isro-lunar-landing-isro-moon-mission-vikram-lander-space-news-11692751244259.html"},{"content":"India Moon Landing In Latest Moon Race, India Aims to Claim First Successful Southern Pole Landing Days after a Russian lunar landing failed, India will try to explore with its...","reference":"https://www.nytimes.com/live/2023/08/23/science/india-moon-landing-chandrayaan-3"}]
```
