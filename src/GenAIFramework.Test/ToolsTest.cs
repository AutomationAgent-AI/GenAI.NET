using Automation.GenerativeAI;
using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GenAIFramework.Test
{
    [TestClass]
    public class ToolsTest : TestBase
    {
        protected override ILanguageModel CreateLanguageModel()
        {
            var responses = new Dictionary<string, string>()
            {
                { "What is the weather like in Boston?", "The weather in Boston is cold" },
                { "What is the weather like in San Francisco?", "The weather in San Francisco is warm" },
                { "sales growth after", "Facebook parent Meta reported a return to sales growth after three quarters of\r\ndeclines." },
                { "imparts wisdom and guides people", "Roger had 5 balls and 2 cans so now he has 9 balls in total." },
                { "looks at answers and finds", "Roger has 5 balls initially and buys 2 cans. Each can contains 3 balls so now he has 5 + 2 x 3 = 11 balls." },
                { "reviews answers and critiques", "Roger has 11 balls" },
            };

            return new MockLanguageModel("Mock", responses);
        }

        public ToolsTest() : base("ToolsTest")
        {
        }

        [TestMethod]
        public void ParseVariableFromSimpleTemplate()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "ParseVariableFromSimpleTemplate");
            var template = @"Tell me a story on the {{ $topic }}";
            var prompt = new PromptTemplate(template);
            Assert.IsNotNull(prompt);
            Assert.AreEqual(Role.user, prompt.Role);
            Assert.AreEqual(1, prompt.Variables.Count);
            Assert.AreEqual("topic", prompt.Variables[0]);
        }

        [TestMethod]
        public void ParseUniqueVariablesFromTemplate()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "ParseUniqueVariablesFromTemplate");

            var template = @"This topic: {{$topic}} is very interested. Tell me more about {{ $topic }}";
            var prompt = new PromptTemplate(template, Role.system);
            Assert.IsNotNull(prompt);
            Assert.AreEqual(Role.system, prompt.Role);
            Assert.AreEqual(1, prompt.Variables.Count);
            Assert.AreEqual("topic", prompt.Variables[0]);
        }

        [TestMethod]
        public void ParseMultipleVariablesFromTemplate()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "ParseMultipleVariablesFromTemplate");

            var template = @"The weather in {{ $city }}, today is {{ $condition }}";
            var prompt = new PromptTemplate(template);
            Assert.IsNotNull(prompt);
            Assert.AreEqual(2, prompt.Variables.Count);
            Assert.AreEqual("city", prompt.Variables[0]);
            Assert.AreEqual("condition", prompt.Variables[1]);
        }

        [TestMethod]
        public void RenderMessage()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "RenderMessage");

            var template = @"The weather in {{ $city }}, today is {{ $condition }}.";
            var prompt = new PromptTemplate(template);
            Assert.IsNotNull(prompt);
            var context = new ExecutionContext();
            context["city"] = "Boston";
            context["condition"] = "cold";

            var msg = prompt.FormatMessage(context);
            Assert.IsNotNull(msg);
            Assert.AreEqual(Role.user.ToString(), msg.role);
            Assert.AreEqual("The weather in Boston, today is cold.", msg.content);
        }

        [TestMethod]
        public async Task TestPromptTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "TestPromptTool");

            var template = @"The weather in {{ $city }}, today is {{ $condition }}.";

            var tool = PromptTool.WithTemplate(template)
                                 .WithName("TestTool")
                                 .WithDescription("Simple Prompt Tool");

            Assert.IsNotNull(tool);
            Assert.AreEqual("TestTool", tool.Name);
            Assert.AreEqual("Simple Prompt Tool", tool.Description);
            var context = new ExecutionContext();
            context["city"] = "Boston";
            context["condition"] = "cold";

            var result = await tool.ExecuteAsync(context);
            Assert.AreEqual("The weather in Boston, today is cold.", result);
            object outcome = string.Empty;
            Assert.IsTrue(context.TryGetResult(tool.Name, out outcome));
            Assert.AreEqual("The weather in Boston, today is cold.", outcome);
        }

        [TestMethod]
        public async Task ToolsPipeline()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "ToolsPipeline");


            var dlltool = new DLLFunctionTools(FunctionsTests.GetDLLPath(), "GenAIFramework.Test.Utilities");
            var funcTool = dlltool.GetTool("AddNumbers");
            Assert.IsNotNull(funcTool);
            Assert.IsTrue(funcTool.Name.Contains("AddNumbers"));

            var promptTool = PromptTool.WithTemplate(@"The addition of two numbers is {{$sum }}.");

            var pipeline = Pipeline.WithTools(new[] { funcTool, promptTool })
                                   .WithName("SimplePipeline")
                                   .WithDescription("Adds two numbers and provides result");

            Assert.IsNotNull(pipeline);
            Assert.AreEqual("SimplePipeline", pipeline.Name);
            Assert.AreEqual("Adds two numbers and provides result", pipeline.Description);

            var context = new ExecutionContext();
            context["a"] = 5.4;
            context["b"] = 4.8;

            var result = await pipeline.ExecuteAsync(context);
            Assert.AreEqual("The addition of two numbers is 10.2.", result);
        }

        [TestMethod]
        public async Task TestQueryTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "TestQueryTool");

            var tool = QueryTool.WithPromptTemplate(@"What is the weather like in {{ $city }}?")
                                .WithLanguageModel(LanguageModel);

            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context["city"] = "Boston";

            var result = await tool.ExecuteAsync(context);
            Assert.AreEqual("The weather in Boston is cold", result);
            object outcome = string.Empty;
            Assert.IsTrue(context.TryGetResult(tool.Name, out outcome));
            Assert.AreEqual("The weather in Boston is cold", outcome);

            context["city"] = "San Francisco";
            result = await tool.ExecuteAsync(context);
            Assert.AreEqual("The weather in San Francisco is warm", result);
        }

        [TestMethod]
        public async Task PDFTextExtractionTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "PDFTextExtractionTool");
            var tool = TextExtractorTool.Create();
            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context["input"] = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\Chandralekha.pdf");
            var result = await tool.ExecuteAsync(context);
            Assert.IsTrue(result.StartsWith("Chandralekha"));
            Assert.IsTrue(result.Contains("Mayabazar"));
            Assert.AreEqual(38918, result.Length);
        }

        [TestMethod]
        public void TextSplitterTest()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "TextSplitterTest");
            
            var txtservice = Application.GetTextProviderService();
            Assert.IsNotNull(txtservice);

            var source = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.txt");
            var textObject = txtservice.GetAllText(source, "English");
            Assert.IsNotNull(textObject);

            var chunkSize = 200.0;
            var overlapLen = 20;

            var splitter = TextSplitter.WithParameters((int)chunkSize, overlapLen);
            Assert.IsNotNull(splitter);

            var splits = splitter.Split(textObject).ToList();
            Assert.IsNotNull(splits);

            Assert.AreEqual(26, splits.Count);
            var overlap = string.Empty;

            var minchunksize = chunkSize - overlapLen;
            foreach (var split in splits)
            {
                Assert.IsTrue(split.Text.Length <= chunkSize);
                if(!string.IsNullOrEmpty(overlap))
                {
                    var substring = split.Text.Substring(0, overlap.Length);
                    Assert.AreEqual(overlap, substring);
                }

                var words = split.Text.Split(' ');
                int len = 0;
                overlap = string.Join(" ", words.Reverse().TakeWhile(s =>
                {
                    len += s.Length+1;
                    return len <= overlapLen + 1;
                }).Reverse());
            }
        }

        [TestMethod]
        public async Task FolderTextExtractionTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "FolderTextExtractionTool");
            var tool = TextExtractorTool.Create();
            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context["input"] = Path.Combine(RootPath, @"..\..\..\..\..\tests\input");
            var result = await tool.ExecuteAsync(context);
            Assert.IsTrue(result.Contains("Chandralekha"));
            Assert.IsTrue(result.Contains("Mayabazar"));
            Assert.IsTrue(result.Contains("4/28/23"));
            Assert.IsTrue(result.Contains("Debra Aho Williamson"));
        }

        [TestMethod]
        public async Task BingSearchTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "BingSearchTool");

            var count = 10;
            var tool = SearchTool.ForBingSearch(string.Empty)
                                 .WithMaxResultCount(count);

            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context[SearchTool.QueryParameter.Name] = "Microsoft and Generative AI";

            var result = await tool.ExecuteAsync(context);
            Assert.IsTrue(!string.IsNullOrEmpty(result));

            var searchresults = FunctionTool.Deserialize<SearchResult[]>(result);
            object obj = null;

            Assert.IsTrue(context.TryGetResult(tool.Name, out obj));
            var oresults = obj as IEnumerable<SearchResult>;
            Assert.AreEqual(oresults.Count(), searchresults.Length);
        }

        [TestMethod]
        public async Task SemanticSearchTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "SemanticSearchTool");

            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var tool = SearchTool.ForSemanticSearchFromDatabase(vdb);
            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context[SearchTool.QueryParameter.Name] = "Meta has reported sales growth after how many quarters of decline?";

            var result = await tool.ExecuteAsync(context);
            Assert.IsTrue(!string.IsNullOrEmpty(result));

            var searchresults = FunctionTool.Deserialize<SearchResult[]>(result);
            Assert.AreEqual(1, searchresults.Length);
        }

        [TestMethod]
        public async Task SemantiSearchAndQueryPipeline()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "SemantiSearchAndQueryPipeline");

            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var tool = SearchTool.ForSemanticSearchFromDatabase(vdb);
            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context[SearchTool.QueryParameter.Name] = "Meta has reported sales growth after how many quarters of decline?";

            var prompt = @"
                        Use following text as context to answer the follow up question.
                        CONTEXT:

                        {{$Result.SemanticSearchTool}}

                        QUESTION:
                        {{$query}}
                        ";

            var querytool = QueryTool.WithPromptTemplate(prompt).WithLanguageModel(LanguageModel);

            var pipeline = Pipeline.WithTools(new IFunctionTool[] { tool, querytool });

            var result = await pipeline.ExecuteAsync(context);

            Assert.IsTrue(!string.IsNullOrEmpty(result));
            Assert.IsTrue(result.Contains("three quarters"));
        }

        [TestMethod]
        public async Task RCIChainPipelineWithInputBinding()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "RCIChainPipelineWithInputBinding");

            var questionPrompt = @"You are a helpful assistant that imparts wisdom and guides people with accurate answers.
Question: {{$question}}
Answer: ";
            var critiquePrompt = @"You are a helpful assistant that looks at answers and finds what is wrong with them based on the original question given.

Question: {{$Input.question}}

Answer Given:{{$initial_answer}}

Review your previous answer and find problems with your answer.";

            var imrpovementPrompt = @"You are a helpful assistant that reviews answers and critiques based on the original question given and write a new improved final answer.

Question: {{$Input.question}}

Answer Given:{{$Result.Query}}

Constructive Criticism:{{$critique}} Based on the problems you found, improve your answer.

Final Answer:";

            //var llm = new OpenAILanguageModel("gpt-3.5-turbo");
            var prompts = new Dictionary<string, string> { { questionPrompt, "Query"}, { critiquePrompt, "Critique" }, { imrpovementPrompt, "Improvise" } };
            var tools = prompts.Select(p => QueryTool.WithPromptTemplate(p.Key).WithLanguageModel(LanguageModel).WithName(p.Value)).ToList();

            var pipeline = Pipeline.WithTools(tools);
            Assert.IsNotNull(pipeline);

            var context = new ExecutionContext();
            context["question"] = "Roger has 5 tenis balls. He buys 2 more cans of tennis balls each with 3 balls. How many tennis balls he has now?";

            var result = await pipeline.ExecuteAsync(context);
            Assert.IsTrue(result.Contains("11"));
        }

        [TestMethod]
        public async Task HttpGet()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "HttpGet");

            using(var client = new HttpClient())
            {
                var tool = HttpTool.WithClient(client);
                Assert.IsNotNull(tool);

                var context = new ExecutionContext();
                context["method"] = "GET";
                context["uri"] = "https://en.wikipedia.org/wiki/Microsoft";

                var response = await tool.ExecuteAsync(context);
                Assert.IsTrue(response.StartsWith("<!DOCTYPE html>"), response);
            }   
        }

        [TestMethod]
        public async Task HttpPost()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "HttpPost");

            using (var client = new HttpClient())
            {
                var tool = HttpTool.WithClient(client);
                Assert.IsNotNull(tool);

                var function = tool.Descriptor;
                Assert.AreEqual(3, function.Parameters.Properties.Count);

                var context = new ExecutionContext();
                string body = "Simple test body";
                context["method"] = "POST";
                context["uri"] = "https://httpbin.org/post";
                context["body"] = body;

                var response = await tool.ExecuteAsync(context);
                Assert.IsTrue(response.StartsWith("{"));
                Assert.IsTrue(response.Contains(body));
            }
        }

        [TestMethod]
        public async Task CharacterCountUsingMapReduce()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "CharacterCountUsingMapReduce");
            var dllpath = GetDLLPath();
            var toolset = new DLLFunctionTools(dllpath, "GenAIFramework.Test.Utilities");

            var lenTool = toolset.GetTool("GetStringLength");
            Assert.IsNotNull(lenTool);

            var sumTool = toolset.GetTool("Sum");
            Assert.IsNotNull(sumTool);

            var mapreduce = MapReduceTool.WithMapperReducer(lenTool, sumTool);
            Assert.IsNotNull(mapreduce);

            var context = new ExecutionContext();
            context["str"] = new[] { "Apple", "Banana", "Mango", "Pear", "Orange" };
            var result = await mapreduce.ExecuteAsync(context);

            Assert.AreEqual("26", result);
        }

        [TestMethod]
        public async Task MultiParameterMapReduce()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "MultiParameterMapReduce");

            var prompt = PromptTool.WithTemplate("The capital of {{$state}} is {{$city}} and '{{$language}}' is the most popular language there.");

            var combine = CombineTool.Create();

            var mapreduce = MapReduceTool.WithMapperReducer(prompt, combine);
            Assert.IsNotNull(mapreduce);

            var context = new ExecutionContext();

            context["state"] = new[] { "UP", "Bihar", "Jharkhand", "MP" };
            context["city"] = new[] { "Lucknow", "Patna", "Ranchi", "Bhopal" };
            context["language"] = new[] { "Hindi", "Bhojpuri", "Santhal", "Hindi" };

            var result = await mapreduce.ExecuteAsync(context);
            Assert.IsTrue(result.Contains("The capital of UP is Lucknow and 'Hindi' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of Bihar is Patna and 'Bhojpuri' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of Jharkhand is Ranchi and 'Santhal' is the most popular language there."));
            Assert.IsTrue(result.Contains("The capital of MP is Bhopal and 'Hindi' is the most popular language there."));
        }

        [TestMethod]
        public async Task SemanticSearchWithContext()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "SemanticSearchWithContext");

            var file = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.txt");
            var context = new ExecutionContext();
            context["context"] = File.ReadAllText(file);
            context["query"] = "Meta has reported sales growth after how many quarters of decline?";

            var tool = SearchTool.ForSemanticSearchFromSource(string.Empty).WithMaxResultCount(3);
            Assert.IsNotNull(tool);

            var result = await tool.ExecuteAsync(context);
            Assert.IsTrue(!string.IsNullOrEmpty(result));

            var searchresults = FunctionTool.Deserialize<SearchResult[]>(result);
            Assert.AreEqual(3, searchresults.Length);
            Assert.IsTrue(searchresults.Any(r => r.content.Contains("three quarter")));
        }
    }
}
