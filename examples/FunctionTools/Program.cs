using Automation.GenerativeAI;
using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FunctionTools
{
    internal class Program
    {
        internal static string GetDLLPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var codebase = asm.CodeBase;
            UriBuilder uri = new UriBuilder(codebase);
            string path = Uri.UnescapeDataString(uri.Path);

            return path;
        }

        static async Task Main(string[] args)
        {
            //await DemoBasicConversationUsingLanguageModel();

            //await DemoConversationWithOpenAIModel();

            //await DemoFunctionCalling();

            //await DemoPromptAndQueryTool();

            //await Task.Run(() => SynchronousPromptAndQueryTool());

            //await DemoPipeline();

            //await Task.Run(() => SynchronousPipeline());

            //await DemoMapReduce();

            //await Task.Run(() => SynchronousMapReduce());

            //await DemoSearchTool();

            //await Task.Run(() => SynchronousBingSearch());

            //await Task.Run(() => CreateFunctionDemo()); 

            //await SimpleChat();
            //await LatestNews();

            //await BrowsePage(@"https://www.google.com/finance/?hl=en");

            //await ExtractData();

            await Task.Run(() => SynchronousExtractData());
        }

        static async Task DemoBasicConversationUsingLanguageModel()
        {
            //Create OpenAI Language model
            var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable
            var llm = new OpenAILanguageModel("gpt-3.5-turbo-0613", apikey);

            //Create Chat Message with a specific role
            var message = new ChatMessage(Role.user, "Hi, there!! I am Ram");

            //Get a Response from the language model using a list of chat messages.
            var response = await llm.GetResponseAsync(Enumerable.Repeat(message, 1), 0.8);
            //response.Response provides the response message
            Console.WriteLine(response.Response);
        }

        static async Task DemoConversationWithOpenAIModel()
        {
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
        }

        static async Task DemoFunctionCalling()
        {
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
        }

        static void CreateFunctionDemo()
        {
            int id = Application.CreateFunctionDescription(
                "GetCurrentWeather", 
                "Gets the current weather information of a city.", 
                "location,The name of a city for which weather information is needed, string");

            var response = Application.GetResponseFromContext("test", "", "", "What is the weather like in Delhi?", 0.8);

            if (response.Contains("function_call"))
            {
                response = Application.AddFunctionMessage("test", "GetCurrentWeather", "{\"temperature\": 22, \"unit\": \"celsius\", \"description\": \"Sunny\"}", 0.8);
            }

            Console.WriteLine(response);
        }

        static async Task DemoPromptAndQueryTool()
        {
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
        }

        static void SynchronousPromptAndQueryTool()
        {
            //Define a template string, the template variables can be denoted as {{ $input }}
            //Following template has two variable adjective and content.
            var template = @"Tell me a {{ $adjective }} joke about {{ $content }}.";

            //Create PromptTemplate with the template string
            var response = Application.CreatePromptTool("JokePrompt", "Renders statement with variables", template);

            Console.WriteLine(response);

            //Define execution context with the parameter values
            var context = "{\"adjective\": \"lame\", \"content\": \"Software Engineer\"}";

            //You can render the message to update the variables in the template string to get the message
            var msg = Application.ExecuteTool("JokePrompt", context); //msg.content == "Tell me a funny joke about chicken."

            Console.WriteLine($"Prompt: {msg}");

            //Create a QueryTool to process this template with the language model
            response = Application.CreateQueryTool("TellAJoke", "Tells a joke!", template);

            Console.WriteLine(response);

            var result = Application.ExecuteTool("TellAJoke", context);

            Console.WriteLine($"Joke: {result}");
        }

        static async Task DemoPipeline()
        {
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
        }

        static void SynchronousPipeline()
        {
            //Define a couple of template strings
            var template1 = "Provide me an engaging title for a blog on topic '{{$topic}}' for '{{$audience}}'.";
            var template2 = "Give me outline of a blog with title: {{$title}}.";

            //Create QueryTool with these prompt templates
            var tools = new List<string> { "T1", "T2" };

            var response = Application.CreateQueryTool(tools[0], "Query Tool", template1);
            Console.WriteLine(response);

            response = Application.CreateQueryTool(tools[1], "Query Tool", template2);
            Console.WriteLine(response);

            //create pipeline with tools
            response = Application.CreateToolsPipeline("Pipeline", "Pipeline Tool", tools);

            //Define execution context with the parameter values
            var context = "{\"topic\": \"Generative AI\", \"audience\": \"Children\"}";

            var outline = Application.ExecuteTool("Pipeline", context); //returns the complete outline for the Blog

            var title = Application.GetExecutionResult(tools[0]);
            Console.WriteLine($"TITLE: {title}");
            Console.WriteLine();
            Console.WriteLine(outline);
        }

        static async Task DemoMapReduce()
        {
            //Prompt tool that takes 3 input variables to create a sentence.
            var prompt = PromptTool.WithTemplate("The capital of {{$state}} is {{$city}} and '{{$language}}' is the most popular language there.");

            //Combines a given array of text by joining the text with a new line.
            var combine = CombineTool.Create();
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
        }

        static void SynchronousMapReduce()
        {
            //Prompt tool that takes 3 input variables to create a sentence.
            var template = "The capital of {{$state}} is {{$city}} and '{{$language}}' is the most popular language there.";

            var response = Application.CreatePromptTool("Prompt", "Prompt Tool", template);
            Console.WriteLine($"Prompt tool creation status: {response}");

            //Combines a given array of text by joining the text with a new line.
            response = Application.CreateCombineTool("Combine", "Combine Tool");
            Console.WriteLine($"Combine tool creation status: {response}");

            //Create a MapReduce tool
            response = Application.CreateMapReduceTool("MapReduce", "MapReduce Tool", "Prompt", "Combine");
            Console.WriteLine($"MapReduce tool creation status: {response}");

            var context = "{\"state\": [ \"UP\", \"Bihar\", \"Jharkhand\", \"MP\" ],\"city\": [ \"Lucknow\", \"Patna\", \"Ranchi\", \"Bhopal\" ],\"language\": [ \"Hindi\", \"Bhojpuri\", \"Santhal\", \"Hindi\" ]\r\n}";

            var result = Application.ExecuteTool("MapReduce", context); //returns the combined result

            Console.WriteLine(result);
            Console.ReadLine();
        }

        static async Task DemoSearchTool()
        {
            //Get Bing API key
            string apiKey = "";

            //Create Bing Search tool with max result count 5
            var tool = SearchTool.ForBingSearch(apiKey).WithMaxResultCount(5);

            var context = new ExecutionContext();
            context[tool.Descriptor.InputParameters.First()] = "What is the latest update on India's moon mission?";

            //Execute the search query
            var result = await tool.ExecuteAsync(context);

            Console.WriteLine(result);
        }

        static void SynchronousBingSearch()
        {
            //Get Bing API key
            string apiKey = "";

            //Create Bing Search tool with max result count 5
            var status = Application.CreateBingSearchTool("Bing", "Bing Search", apiKey, 5);

            Console.WriteLine($"Bing Search Tool Creation Status: {status}");

            var inputparameters = Application.GetToolInputParameters("Bing");
            Console.WriteLine(inputparameters.First());

            var context = "{\"query\": \"What is the latest update on India's moon mission?\"}";

            //Execute the search query
            var result = Application.ExecuteTool("Bing", context);

            Console.WriteLine(result);
        }

        static async Task ExtractData()
        {
            var path = @"<<YOUR DOCUMENT PATH>>";
            var txt = TextExtractorTool.ExtractText(path);

            var parameters = new Dictionary<string, string>() 
            {
                { "Customer Name", "What is the name of the company sending this Purchase Order." },
                { "Supplier Name", "What is the name of the supplier." },
                { "PO Date", "What is the Date on which the purchase order was issued." },
                { "PO No.", "What is the purchase order number." },
                { "Total Amount", "What is the total amount of the PO" },
                { "Consignee", "Who is the consignee?" },
                { "Shipping Term", "What is the frieght basis?" },
                { "Item Desription", "What is the description of the item ordered here? Include the packaging details, material number etc." },
                { "Qunatity", "What is the quantity of the item ordered?" },
                { "Unit Price", "What is the unit price of the item ordered?" },
            };

            var extractor = DataExtractorTool.Create().WithParameters(parameters);

            var results = await extractor.ExtractDataAsync(txt);

            foreach (var pair in results)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }

            Console.ReadLine();
        }

        static void SynchronousExtractData()
        {
            var path = @"<<YOUR DOCUMENT PATH>>";

            //Create text extractor tool
            var status = Application.CreateTextExtractorTool("ExtractText", "Extracts text");

            Console.WriteLine($"ExtractText Tool Creation Status: {status}");
            
            var parameters = new Dictionary<string, string>()
            {
                { "Customer Name", "What is the name of the company sending this Purchase Order." },
                { "Supplier Name", "What is the name of the supplier." },
                { "PO Date", "What is the Date on which the purchase order was issued." },
                { "PO No.", "What is the purchase order number." },
                { "Total Amount", "What is the total amount of the PO" },
                { "Consignee", "Who is the consignee?" },
                { "Shipping Term", "What is the frieght basis?" },
                { "Item Desription", "What is the description of the item ordered here? Include the packaging details, material number etc." },
                { "Qunatity", "What is the quantity of the item ordered?" },
                { "Unit Price", "What is the unit price of the item ordered?" },
            };

            var prametersJson = FunctionTool.ToJsonString(parameters);

            status = Application.CreateDataExtractorTool("DataExtractor", "Extracts Data", prametersJson);
            Console.WriteLine($"DataExtractor Tool Creation Status: {status}");

            var tools = new List<string>() { "ExtractText", "DataExtractor" };
            status = Application.CreateToolsPipeline("DataPipeline", "Data extraction pipeline", tools);
            Console.WriteLine($"DataPipeline Tool Creation Status: {status}");

            var dict = new Dictionary<string, string>() { { "source", path } };
            var ctx = FunctionTool.ToJsonString(dict);
            var results = Application.ExecuteTool("DataPipeline", ctx);

            var data = FunctionTool.Deserialize<Dictionary<string, string>>(results);

            foreach (var pair in data)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value}");
            }

            Console.ReadLine();
        }

        static async Task BrowsePage(string url)
        {
            //Create function tool from DLL and class name
            var toolset = new DLLFunctionTools(@"GenerativeAI.Tools.dll", "");

            var extractor = toolset.GetTool("GetTextContentFromWebpage");

            var context = new ExecutionContext();
            context[extractor.Descriptor.InputParameters.First()] = url;
            var text = await extractor.ExecuteAsync(context);

            Console.Write(text);
            Console.ReadLine();
        }

        static async Task LatestNews()
        {
            //Create GenAI service
            var service = Application.GetAIService();

            var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable

            //Create Language Model
            var llm = service.CreateOpenAIModel("gpt-3.5-turbo-0613", apikey);

            //Create Conversation
            var chat = service.CreateConversation("Test", llm);

            var dllpath = GetDLLPath();
            var foldername = Path.GetDirectoryName(dllpath);

            var logfile = Path.Combine(foldername, "sample.log");
            Application.SetLogFilePath(logfile);

            //Create function tool from DLL and class name
            var toolset = new DLLFunctionTools(@"GenerativeAI.Tools.dll", "");

            var extractor = toolset.GetTool("GetTextContentFromWebpage");
            var bing = SearchTool.ForBingSearch(string.Empty).WithMaxResultCount(3);

            //var tools = new ToolsCollection(new[] { extractor, bing });
            var tools = new ToolsCollection(bing);

            //Add toolset to the conversation
            chat.AddToolSet(tools);

            var msg = @"You are an intelligent assistant performs thourough research on any given query. 
                  Think step by step and analyze the input 
                  request to check if any function call is required, if so extract all
                  parameters based on the function sepcification. Extract arguments and values
                  only based on function specification provided, do not include extra parameter. If required feel
                  free to browse a given link to get more insight on the available data.";

            //chat.AppendMessage(msg, Role.system);

            //Add your question
            chat.AppendMessage(
                @"Provide me the latest update on Chandrayan 3 mission with key timelines, challenges and how it overcame those challenges. Keep the informations in chronological order.", Role.user);

            //Get response from chat
            var response = await chat.GetResponseAsync(0.5);

            Console.WriteLine($"{response.role}: {response.content}");
        }

        static async Task SimpleChat()
        {
            //Create GenAI service
            var service = Application.GetAIService();

            var apikey = ""; //Pass your API key, or set your api key to OPENAI_API_KEY environment variable

            //Create Language Model
            var llm = service.CreateOpenAIModel("gpt-3.5-turbo-0613", apikey);
            
            //Create Conversation
            var chat = service.CreateConversation("Test", llm);

            var dllpath = GetDLLPath();

            var logfile = Path.Combine(Path.GetDirectoryName(dllpath), "sample.log");
            Application.SetLogFilePath(logfile);

            //Create function tool from DLL and class name
            var tool = new DLLFunctionTools(dllpath, "FunctionTools.Utilities");

            //Add toolset to the conversation
            chat.AddToolSet(tool);

            //Add your question
            chat.AppendMessage(
                @"Please do three things, add an amount of 40 units to year 2023 headcount 
                  and subtract an amount of 23 units from year 2022 opex forecast then 
                  print out the forecast at home", Role.user);

            //Get response from chat
            var response = await chat.GetResponseAsync(0.5);

            Console.WriteLine($"{response.role}: {response.content}");
        }
    }
}
