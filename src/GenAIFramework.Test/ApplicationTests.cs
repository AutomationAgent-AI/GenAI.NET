using Automation.GenerativeAI;
using Automation.GenerativeAI.Agents;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GenAIFramework.Test
{
    [TestClass]
    public class ApplicationTests : TestBase
    {
        protected override ILanguageModel CreateLanguageModel()
        {
            var responses = new Dictionary<string, string>()
            {
                { "What is the weather like in Boston?", "The weather in Boston is cold" },
                { "What is the weather like in San Francisco?", "The weather in San Francisco is warm" },
                { "sales growth after", "Facebook parent Meta reported a return to sales growth after three quarters of declines." },
                { "add an amount of 40 units to year 2023 headcount", "{\"name\": \"EditFinancialForecast\",\"arguments\": \"{ \\\"year\\\": 2023, \\\"category\\\": \\\"headcount\\\", \\\"amount\\\": 40}\"}" },
                { "{\"headcount\":240,\"opex\":500}", "{\"name\": \"EditFinancialForecast\",\"arguments\": \"{ \\\"year\\\": 2023, \\\"category\\\": \\\"opex\\\", \\\"amount\\\": -23}\"}"},
                { "{\"headcount\":240,\"opex\":477}", "{\"name\": \"PrintFinancialForecast\",\"arguments\": \"{ \\\"printer\\\": \\\"HomePrinter\\\"}\"}"},
                { "Printed the forecast to", "Updated the financial forecast and printed at home" }
            };

            var model = new MockLanguageModel("Mock", responses);
            return model;
        }

        [TestInitialize]
        public void Setup()
        {
            Application.SetLanguageModel(LanguageModel);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Application.Reset();
            Utilities.Reset();
        }

        public ApplicationTests() : base("ApplicationTests") { }

        [TestMethod]
        public void CreatePromptTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreatePromptTool");

            var template = @"The weather in {{ $city }}, today is {{ $condition }}.";

            string name = "TestTool";
            var status = Application.CreatePromptTool(name, "Simple prompt tool", template);

            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["city"] = "Boston";
            context["condition"] = "cold";

            var result = Application.ExecuteTool(name, FunctionTool.ToJsonString(context));
            Assert.AreEqual("The weather in Boston, today is cold.", result);

            var outcome = Application.GetExecutionResult(name); ;
            Assert.AreEqual("The weather in Boston, today is cold.", outcome);
        }

        [TestMethod]
        public void CreateToolsPipeline()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateToolsPipeline");

            var tools = Application.AddToolsFromDLL(GetDLLPath());
            Assert.IsTrue(tools.Count > 0);

            string prompt = "PromptTool";

            var status = Application.CreatePromptTool(prompt, "Prompt tool", @"The addition of two numbers is {{$sum }}.");
            Assert.AreEqual("success", status);

            string pipeline = "SimplePipeline";
            string description = "Adds two numbers and provides result";
            status = Application.CreateToolsPipeline(pipeline, description, new List<string>() {"AddNumbers", prompt});

            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["a"] = 5.4;
            context["b"] = 4.8;

            var result = Application.ExecuteTool(pipeline, FunctionTool.ToJsonString(context));
            Assert.AreEqual("The addition of two numbers is 10.2.", result);
        }

        [TestMethod]
        public void CreateQueryTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateQueryTool");
            var tool = "QueryTool";
            var status = Application.CreateQueryTool(tool, "Query tool", @"What is the weather like in {{ $city }}?");
            Assert.AreEqual("success", status);

            var context = "{ \"city\": \"Boston\" }";
            
            var result = Application.ExecuteTool(tool, context);
            Assert.AreEqual("The weather in Boston is cold", result);
            object outcome = Application.GetExecutionResult(tool);
            Assert.AreEqual("The weather in Boston is cold", outcome);

            context = "{ \"city\": \"San Francisco\" }";
            result = Application.ExecuteTool(tool, context);
            Assert.AreEqual("The weather in San Francisco is warm", result);
        }

        [TestMethod]
        public void CreatePDFTextExtractionTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreatePDFTextExtractionTool");
            var tool = "TextExtractor";

            var status = Application.CreateTextExtractorTool(tool, "Extracts text");
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["input"] = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\Chandralekha.pdf");
            var result = Application.ExecuteTool(tool, FunctionTool.ToJsonString(context));
            Assert.IsTrue(result.StartsWith("Chandralekha"));
            Assert.IsTrue(result.Contains("Mayabazar"));
            Assert.AreEqual(38918, result.Length);
        }

        [TestMethod]
        public void CreateFolderTextExtractionTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateFolderTextExtractionTool");

            var tool = "FolderExtractor";
            var status = Application.CreateTextExtractorTool(tool, "Extracts text");
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["input"] = Path.Combine(RootPath, @"..\..\..\..\..\tests\input");
            var result = Application.ExecuteTool(tool, FunctionTool.ToJsonString(context));
            Assert.IsTrue(result.Contains("Chandralekha"));
            Assert.IsTrue(result.Contains("Mayabazar"));
            Assert.IsTrue(result.Contains("4/28/23"));
            Assert.IsTrue(result.Contains("Debra Aho Williamson"));
        }

        [TestMethod]
        public void CreateBingSearchTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateBingSearchTool");

            var tool = "BingSearchTool";
            var count = 10;
            var status = Application.CreateBingSearchTool(tool, "Search", string.Empty, count);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context[SearchTool.QueryParameter.Name] = "Microsoft and Generative AI";

            var result = Application.ExecuteTool(tool,FunctionTool.ToJsonString(context));
            Assert.IsTrue(!string.IsNullOrEmpty(result));

            var searchresults = FunctionTool.Deserialize<SearchResult[]>(result);
            Assert.IsNotNull(searchresults);
            Assert.IsTrue(searchresults.Count() > 0);

            var objresults = Application.GetExecutionResult(tool);
            Assert.AreEqual(objresults, result);
        }

        [TestMethod]
        public void CreateSemanticSearchTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateSemanticSearchTool");

            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var tool = "SemanticSearch";
            var status = Application.CreateSemanticSearchTool(tool, "for search", vdb, 1);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context[SearchTool.QueryParameter.Name] = "Meta has reported sales growth after how many quarters of decline?";

            var result = Application.ExecuteTool(tool, FunctionTool.ToJsonString(context));
            Assert.IsTrue(!string.IsNullOrEmpty(result));

            var searchresults = FunctionTool.Deserialize<SearchResult[]>(result);
            Assert.AreEqual(1, searchresults.Length);
        }

        [TestMethod]
        public void CreateSemantiSearchQueryPipeline()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateSemantiSearchQueryPipeline");

            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var tool = "SemanticSearchTool";

            var status = Application.CreateSemanticSearchTool(tool, "for search", vdb, 1);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context[SearchTool.QueryParameter.Name] = "Meta has reported sales growth after how many quarters of decline?";

            var prompt = @"
                        Use following text as context to answer the follow up question.
                        CONTEXT:

                        {{$Result.SemanticSearchTool}}

                        QUESTION:
                        {{$query}}
                        ";

            var querytool = "QueryTool";
            status = Application.CreateQueryTool(querytool, "Answers to your query", prompt);
            Assert.AreEqual("success", status);

            status = Application.CreateToolsPipeline("pipeline", "query pipeline", new List<string>() { tool, querytool });
            Assert.AreEqual("success", status);

            var result = Application.ExecuteTool("pipeline", FunctionTool.ToJsonString(context));

            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("three quarters"));
        }

        [TestMethod]
        public void CreateHttpTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateHttpTool");

            var tool = "HttpGet";
            var status = Application.CreateHttpRequestTool(tool, "Makes HTTP request", string.Empty);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["method"] = "GET";
            context["uri"] = "https://en.wikipedia.org/wiki/Microsoft";

            var response = Application.ExecuteTool(tool, FunctionTool.ToJsonString(context));
            Assert.IsTrue(response.StartsWith("<!DOCTYPE html>"), response);
        }

        [TestMethod]
        public void CreateHttpPost()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateHttpPost");

            var tool = "HttpPost";
            var status = Application.CreateHttpRequestTool(tool, "HTTP Post Request", string.Empty);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            string body = "Simple test body";
            context["method"] = "POST";
            context["uri"] = "https://httpbin.org/post";
            context["body"] = body;

            var response = Application.ExecuteTool(tool, FunctionTool.ToJsonString(context));
            Assert.IsTrue(response.StartsWith("{"));
            Assert.IsTrue(response.Contains(body));
        }

        [TestMethod]
        public void CreateMapReduceTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateMapReduceTool");
            var dllpath = GetDLLPath();
            var toolset = Application.AddToolsFromDLL(dllpath);

            Assert.IsTrue(toolset.Count > 0);

            var mapper = "GetStringLength";
            var reducer = "Sum";

            var mapreduce = "MapReduce";

            var status = Application.CreateMapReduceTool(mapreduce, "Map Reduce tool for Character count", mapper, reducer);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();
            context["str"] = new[] { "Apple", "Banana", "Mango", "Pear", "Orange" };
            var result = Application.ExecuteTool(mapreduce, FunctionTool.ToJsonString(context));

            Assert.AreEqual("26", result);
        }

        [TestMethod]
        public void CreateMultiParameterMapReduce()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateMultiParameterMapReduce");

            var prompt = "The capital of {{$state}} is {{$city}} and '{{$language}}' is the most popular language there.";

            var mapper = "PromptTool";
            var reducer = "CombineTool";

            var status = Application.CreatePromptTool(mapper, "Prompt tool", prompt);
            Assert.AreEqual("success", status);

            status = Application.CreateCombineTool(reducer, "Combines text");
            Assert.AreEqual("success", status);

            var mapreduce = "MapReduceTool";

            status = Application.CreateMapReduceTool(mapreduce, "Map Reduce tool", mapper, reducer);
            Assert.AreEqual("success", status);

            var context = new Dictionary<string, object>();

            context["state"] = new[] { "UP", "Bihar", "Jharkhand", "MP" };
            context["city"] = new[] { "Lucknow", "Patna", "Ranchi", "Bhopal" };
            context["language"] = new[] { "Hindi", "Bhojpuri", "Santhal", "Hindi" };

            var result = Application.ExecuteTool(mapreduce, FunctionTool.ToJsonString(context));
            Assert.IsTrue(result.Contains("The capital of UP is Lucknow and 'Hindi' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of Bihar is Patna and 'Bhojpuri' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of Jharkhand is Ranchi and 'Santhal' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of MP is Bhopal and 'Hindi' is the most popular language there."));
        }

        [TestMethod]
        public void CreateAgentWithObjective()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CreateAgentWithObjective");

            var dllpath = FunctionsTests.GetDLLPath();
            var tools = Application.AddToolsFromDLL(dllpath);

            var agent = "ForecastAgent";
            
            var status = Application.CreateAgent(agent, tools, 3, string.Empty);
            Assert.AreEqual("success", status);

            var objective = @"Please do three things, add an amount of 40 units to year 2023 headcount 
                  and subtract an amount of 23 units from year 2022 opex forecast then 
                  print out the forecast at home";

            var action = Application.PlanAndExecuteWithAgent(agent, objective, 0.8);
            Assert.IsNotNull(action);

            var step = FunctionTool.Deserialize<StepAction>(action);
            Assert.IsNotNull(step);

            Assert.AreEqual("FinishAction", step.tool);
            var output = step.parameters["output"].ToString().ToLower();
            Assert.IsTrue(output.Contains("updated") || output.Contains("added"));
            Assert.IsTrue(output.Contains("printed"));
            Assert.IsTrue(output.Contains("home"));
        }

        [TestMethod]
        public void AgentWithClientManagedTools()
        {
            var parameters = @"year,The year for which data to edit,int
                               category,The category of data to edit,string
                               amount,Amount of units to edit,int";

            //Create function description without any execution logic.
            var status = Application.CreateToolDescriptor("EditFinancialForecast", "Makes an edit to users financial forecast model.", parameters);
            Assert.AreEqual("success", status);

            parameters = @"printer,Printer name";
            //Create function description without any execution logic.
            status = Application.CreateToolDescriptor("PrintFinancialForecast", "Sends the financial forecast for print", parameters);
            Assert.AreEqual("success", status);

            var tools = new List<string>() { "EditFinancialForecast", "PrintFinancialForecast" };
            var agent = "ForecastAgent";

            status = Application.CreateAgent(agent, tools, 3, string.Empty);
            Assert.AreEqual("success", status);

            var objective = @"Please do three things, add an amount of 40 units to year 2023 headcount 
                  and subtract an amount of 23 units from year 2022 opex forecast then 
                  print out the forecast at home";

            var action = Application.PlanAndExecuteWithAgent(agent, objective, 0.8);
            Assert.IsNotNull(action);

            var step = FunctionTool.Deserialize<StepAction>(action);
            Assert.IsNotNull(step);
            Assert.AreEqual("EditFinancialForecast", step.tool);
            Assert.AreEqual(2023, step.parameters["year"]);
            Assert.AreEqual("headcount", step.parameters["category"]);
            Assert.AreEqual(40, step.parameters["amount"]);

            action = Application.UpdateAgentActionResponse(agent, step.tool, "{\"headcount\":240,\"opex\":500}");
            Assert.IsNotNull(action);

            step = FunctionTool.Deserialize<StepAction>(action);
            Assert.IsNotNull(step);
            Assert.AreEqual("EditFinancialForecast", step.tool);
            Assert.AreEqual(2023, step.parameters["year"]);
            Assert.AreEqual("opex", step.parameters["category"]);
            Assert.AreEqual(-23, step.parameters["amount"]);

            action = Application.UpdateAgentActionResponse(agent, step.tool, "{\"headcount\":240,\"opex\":477}");
            Assert.IsNotNull(action);

            step = FunctionTool.Deserialize<StepAction>(action);
            Assert.IsNotNull(step);
            Assert.AreEqual("PrintFinancialForecast", step.tool);

            action = Application.UpdateAgentActionResponse(agent, step.tool, "Printed the forecast to home printer.");
            Assert.IsNotNull(action);

            step = FunctionTool.Deserialize<StepAction>(action);
            Assert.AreEqual("FinishAction", step.tool);
            var output = step.parameters["output"].ToString().ToLower();
            Assert.IsTrue(output.Contains("updated") || output.Contains("added"));
            Assert.IsTrue(output.Contains("printed"));
            Assert.IsTrue(output.Contains("home"));
        }
    }
}
