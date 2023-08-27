using Automation.GenerativeAI;
using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GenerativeApp = Automation.GenerativeAI.Application;

namespace GenAIFramework.Test
{
    internal class SimpleTool : IFunctionToolSet
    {
        public IFunctionTool this[string name] => throw new System.NotImplementedException();

        public string Execute(string functionName, ExecutionContext context)
        {
            if (functionName.Equals("get_current_weather"))
            {
                return get_current_weather((string)context["location"]);
            }

            return null;
        }

        public async Task<string> ExecuteAsync(string functionName, ExecutionContext context)
        {
            return await Task.Run(()=>Execute(functionName, context));
        }

        public IEnumerator<IFunctionTool> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<FunctionDescriptor> GetFunctions()
        {
            var function = FunctionsTests.CreateSampleFunction();
            return Enumerable.Repeat(function, 1);
        }

        public IFunctionTool GetTool(string functionName)
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        private string get_current_weather(string location)
        {
            if (location.Contains("Boston"))
            {
                return "{\\\"temperature\\\": \\\"22\\\", \\\"unit\\\": \\\"celsius\\\", \\\"description\\\": \\\"Sunny\\\"}";
            }
            else if(location.Contains("San Francisco"))
            {
                return "{\\\"current temperature\\\": \\\"18.5\\\", \\\"unit\\\": \\\"celsius\\\", \\\"description\\\": \\\"Cloudy\\\"}";
            }
            else
            {
                return string.Empty; //can't execute
            }
        }
    }

    [TestClass]
    public class ConversationTests
    {
        private string RootPath = string.Empty;
        private ILanguageModel languageModel;

        public ConversationTests()
        {
            RootPath = Assembly.GetExecutingAssembly().Location;
            var logfile = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\ConversationTests.log");
            Logger.SetLogFile(logfile);

            var responses = new Dictionary<string, string>()
            {
                { "Hi, there!! I am Ram", "Hello Ram! How can I assist you today?"},
                { "What is my name and which fruit do I like?", "Your name is Romeo and you like to eat apples for breakfast."},
                { "Software Engineer, What do I do for living?","You work as a Software Engineer." },
                { "What is the weather like in Boston?", "{\"name\": \"get_current_weather\",\"arguments\": \"{ \\\"location\\\": \\\"Boston, MA\\\"}\"}" },
                { "What is the weather like in San Francisco?", "{\"name\": \"get_current_weather\",\"arguments\": \"{ \\\"location\\\": \\\"San Francisco, CA\\\"}\"}" },
                { "What is the weather like in Timbaktu?", "{\"name\": \"get_current_weather\",\"arguments\": \"{ \\\"location\\\": \\\"Timbaktu\\\"}\"}" },
                { "current temperature 18.5 unit celsius description Cloudy", "The current weather in San Francisco is cloudy with a temperature of 18.5 degrees Celsius." },
                { "Timbaktu temperature", "The current temperature in Timbaktu is 20 degrees Celsius." },
                { "temperature 22 unit celsius description Sunny", "The current weather in Boston is sunny with a temperature of 22 degrees Celsius." },
                { "add an amount of 40 units to year 2023 headcount", "{\"name\": \"EditFinancialForecast\",\"arguments\": \"{ \\\"year\\\": 2023, \\\"category\\\": \\\"headcount\\\", \\\"amount\\\": 40}\"}" },
                { "{\"headcount\":240,\"opex\":500}", "{\"name\": \"EditFinancialForecast\",\"arguments\": \"{ \\\"year\\\": 2023, \\\"category\\\": \\\"opex\\\", \\\"amount\\\": -23}\"}"},
                { "{\"headcount\":240,\"opex\":477}", "{\"name\": \"PrintFinancialForecast\",\"arguments\": \"{ \\\"printer\\\": \\\"HomePrinter\\\"}\"}"},
                { "Printed the forecast to", "Updated the financial forecast and printed at home" }
            };

            languageModel = new MockLanguageModel("Mock", responses);
            //languageModel = new OpenAILanguageModel("gpt-3.5-turbo-0613");
            GenerativeApp.SetLanguageModel(languageModel);
        }

        [TestMethod]
        public async Task HelloWorldChat()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "HelloWorldChat");
            
            var chat = new Conversation("hello", languageModel);
            Assert.IsNotNull(chat);
            Assert.AreEqual("hello", chat.Id);

            chat.AppendMessage("Hi, there!! I am Ram", Role.user);

            var response = await chat.GetResponseAsync(0.8);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("Ram"));
        }

        [TestMethod]
        public async Task BasicFunctionTool()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "BasicFunctionTool");
            var chat = new Conversation("tool", languageModel);
            Assert.IsNotNull(chat);
            Assert.AreEqual("tool", chat.Id);

            var tool = new SimpleTool();
            chat.AddToolSet(tool);

            chat.AppendMessage("What is the weather like in Boston?", Role.user);
            var response = await chat.GetResponseAsync(0.8);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("22 degrees Celsius"));
            Assert.IsTrue(response.content.Contains("weather in Boston"));
        }

        [TestMethod]
        public async Task BasicFunctionToolExecution()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "BasicFunctionToolExecution");
            var chat = new Conversation("test", languageModel);

            var tool = new SimpleTool();
            chat.AddToolSet(tool);
            chat.AppendMessage("What is the weather like in San Francisco?", Role.user);
            var response = await chat.GetResponseAsync(0.5);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("18.5 degrees Celsius"));
            Assert.IsTrue(response.content.Contains("cloudy"));
        }

        [TestMethod]
        public async Task FailedFunctionToolExecution()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "FailedFunctionToolExecution");
            var chat = new Conversation("test", languageModel);

            var tool = new SimpleTool();
            chat.AddToolSet(tool);
            chat.AppendMessage("What is the weather like in Timbaktu?", Role.user);
            var response = await chat.GetResponseAsync(0.5);
            Assert.AreEqual("assistant", response.role);

            //The SimpleTool doesn't return any value for location Timbaktu,
            //hence it fails and the chat returns a FunctionCallMessage.
            Assert.IsTrue(response is FunctionCallMessage);
        }

        [TestMethod]
        public async Task DLLFunctionToolExecution()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "DLLFunctionToolExecution");
            var chat = new Conversation("test", languageModel);

            var dllpath = FunctionsTests.GetDLLPath();
            var tool = new DLLFunctionTools(dllpath, "GenAIFramework.Test.Utilities");

            chat.AddToolSet(tool);
            chat.AppendMessage("What is the weather like in San Francisco?", Role.user);
            var response = await chat.GetResponseAsync(0.5);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("18.5 degrees Celsius"), response.content);
            Assert.IsTrue(response.content.Contains("cloudy"), response.content);
        }

        [TestMethod]
        public async Task MultipleFunctionCall()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "MultipleFunctionCall");
            var chat = new Conversation("test", languageModel);

            var dllpath = FunctionsTests.GetDLLPath();
            var tool = new DLLFunctionTools(dllpath, "GenAIFramework.Test.Utilities");

            chat.AddToolSet(tool);

            //Add your question
            chat.AppendMessage(
                @"Please do three things, add an amount of 40 units to year 2023 headcount 
                  and subtract an amount of 23 units from year 2022 opex forecast then 
                  print out the forecast at home", Role.user);

            var response = await chat.GetResponseAsync(0.8);
            Assert.IsNotNull(response);
            var msg = response.content.ToLower();
            Assert.IsTrue(msg.Contains("updated") || msg.Contains("added"));
            Assert.IsTrue(msg.Contains("printed"));
            Assert.IsTrue(msg.Contains("home"));
        }

        [TestMethod]
        public async Task HandleFunctionCallMessage()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "HandleFunctionCallMessage");
            var chat = new Conversation("test", languageModel);

            var tool = new SimpleTool();
            chat.AddToolSet(tool);
            chat.AppendMessage("What is the weather like in Timbaktu?", Role.user);
            var response = await chat.GetResponseAsync(0.5);
            Assert.AreEqual("assistant", response.role);
            Assert.IsTrue(response is FunctionCallMessage);

            var callmsg = (FunctionCallMessage)response;
            var fmsg = new FunctionMessage((string)callmsg.function_call["name"], "Timbaktu temperature: 20 degree Celsius");

            chat.AppendMessage(fmsg);

            response = await chat.GetResponseAsync(0.5);
            Assert.AreEqual("assistant", response.role);
            Assert.IsTrue(response.content.Contains("20 degrees Celsius"));
            Assert.IsTrue(response.content.Contains("Timbaktu"));
        }

        [TestMethod]
        public async Task ContextBasedQuestions()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "ContextBasedQuestions");
            var chat = new Conversation("context", languageModel);

            chat.AppendMessage("What is my name and which fruit do I like?", Role.user);

            var data = new[]
            {
                TextObject.Create("S1", "My name is Romeo"),
                TextObject.Create("S2", "I live at Timbaktu"),
                TextObject.Create("S3", "I work as a Software Engineer"),
                TextObject.Create("S4", "I love to eat apple for breakfast")
            };

            chat.AddContext(data);

            var response = await chat.GetResponseAsync(0.2);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("Romeo"));
            Assert.IsTrue(response.content.Contains("apple"));

            chat.AppendMessage("What do I do for living?", Role.user);
            response = await chat.GetResponseAsync(0.5);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.ToLower().Contains("software"));
        }
    }
}
