using Automation.GenerativeAI;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GenAIFramework.Test
{
    [TestClass]
    public class OpenAITests
    {
        private string RootPath = string.Empty;

        public OpenAITests()
        {
            RootPath = Assembly.GetExecutingAssembly().Location;
            var logfile = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\OpenAITests.log");
            
            if (File.Exists(logfile)) {
                File.Delete(logfile);
            }            
            Logger.SetLogFile(logfile);
        }


        [TestMethod]
        public async Task ValidOpenAIModel()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: ValidOpenAIModel");
            var model = "gpt-3.5-turbo";
            var llm = new OpenAILanguageModel(model);
            Assert.IsNotNull(llm);
            Assert.AreEqual(model, llm.ModelName);
            var message = new ChatMessage(Role.user, "Hi, there!! I am Ram");
            var response = await llm.GetResponseAsync(Enumerable.Repeat(message, 1), 0.8);
            Assert.IsTrue(response.Response.Contains("Ram"));
        }

        [TestMethod]
        public async Task InValidOpenAIModel()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: InValidOpenAIModel");
            var model = "xyz";
            var llm = new OpenAILanguageModel(model);
            Assert.IsNotNull(llm);
            Assert.AreEqual(model, llm.ModelName);
            var message = new ChatMessage(Role.user, "Hi, there!! I am Ram");
            var response = await llm.GetResponseAsync(Enumerable.Repeat(message, 1), 0.8);
            Assert.AreEqual(ResponseType.Failed, response.Type);
        }

        [TestMethod]
        public void FunctionCall()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: FunctionCall");
            var function = FunctionsTests.CreateSampleFunction();
            var model = "gpt-3.5-turbo-0613";
            var llm = new OpenAILanguageModel(model);
            Assert.IsNotNull(llm);
            Assert.AreEqual(model, llm.ModelName);
            var message = new ChatMessage(Role.user, "What is the weather like in Boston?");

            var response = llm.GetResponse(
                Enumerable.Repeat(message, 1),
                Enumerable.Repeat(function, 1),
                0.8);

            Assert.AreEqual(ResponseType.FunctionCall, response.Type);
            Assert.IsTrue(response.Response.Contains("get_current_weather"));
            Assert.IsTrue(response.Response.Contains("location"));
        }

        [TestMethod]
        public void FunctionCallSummarize()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: FunctionCall");
            var function = FunctionsTests.CreateSampleFunction();
            var model = "gpt-3.5-turbo-0613";
            var llm = new OpenAILanguageModel(model);
            Assert.IsNotNull(llm);
            Assert.AreEqual(model, llm.ModelName);
            var usermsg = new ChatMessage(Role.user, "What is the weather like in Boston?");
            var aimsg = new FunctionCallMessage() { 
                function_call = new Dictionary<string, object>() 
                    {
                        { "name", "get_current_weather" },
                        { "arguments", "{ \"location\": \"Boston, MA\"}"}
                    } 
            };
            var funmsg = new FunctionMessage("get_current_weather", "{\"temperature\": \"22\", \"unit\": \"celsius\", \"description\": \"Sunny\"}");
            var messages = new ChatMessage[] {usermsg, aimsg, funmsg };
            var response = llm.GetResponse(
                messages,
                Enumerable.Repeat(function, 1),
                0.8);

            Assert.AreEqual(ResponseType.Done, response.Type);
            Assert.IsTrue(response.Response.Contains("22 degrees Celsius"));
            Assert.IsTrue(response.Response.Contains("Boston"));
        }
    }
}
