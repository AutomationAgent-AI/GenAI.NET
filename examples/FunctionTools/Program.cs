using Automation.GenerativeAI;
using Automation.GenerativeAI.Interfaces;
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
            //await SimpleChat();
            //await LatestNews();

            //await BrowsePage(@"https://www.google.com/finance/?hl=en");

            //await ExtractData();

            await MapReduce();
        }

        static async Task MapReduce()
        {
            var prompt = QueryTool.WithPromptTemplate("What is the capitol of {{$state}} and which language is spoken there?");

            var combine = new CombineTool();

            var mapreduce = MapReduceTool.WithMapperReducer(prompt, combine);

            var context = new ExecutionContext();

            context["state"] = new[] { "UP", "Bihar", "Jharkhand", "MP" };
            context["city"] = new[] { "Lucknow", "Patna", "Ranchi", "Bhopal" };
            context["language"] = new[] { "Hindi", "Bhojpuri", "Santhal", "Hindi" };

            var result = await mapreduce.ExecuteAsync(context);

            Console.WriteLine(result);
            Console.ReadLine();
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
