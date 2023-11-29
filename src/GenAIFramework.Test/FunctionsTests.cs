using Automation.GenerativeAI;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GenAIFramework.Test
{
    [TestClass]
    public class FunctionsTests
    {
        private string RootPath = string.Empty;
        
        public FunctionsTests()
        {
            RootPath = Assembly.GetExecutingAssembly().Location;
            var logfile = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\FunctionsTests.log");
            Logger.SetLogFile(logfile);
        }

        internal static FunctionDescriptor CreateSampleFunction()
        {
            var p1 = new ParameterDescriptor()
            {
                Name = "location",
                Description = "The city and state, e.g. San Francisco, CA",
                Required = true,
                Type = TypeDescriptor.StringType
            };
            var p2 = new ParameterDescriptor()
            {
                Name = "unit",
                Description = string.Empty,
                Required = false,
                Type = new EnumTypeDescriptor(new string[] { "celsius", "fahrenheit" })
            };
            var parameters = new List<ParameterDescriptor>() { p1, p2 };

            var function = new FunctionDescriptor("get_current_weather",
                "Get the current weather in a given location",
                parameters);

            return function;
        }

        [TestMethod]
        public void FunctionSerialization()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: FunctionSerialization");
            var function = CreateSampleFunction();
            var json = OpenAIClient.ToJSON(Enumerable.Repeat(function, 1));
            Assert.IsNotNull(json);
            var expected = "[{\"name\":\"get_current_weather\",\"description\":\"Get the current weather in a given location\",\"parameters\":{\"type\":\"object\",\"properties\":{\"location\":{\"type\":\"string\",\"description\":\"The city and state, e.g. San Francisco, CA\"},\"unit\":{\"type\":\"string\",\"enum\":[\"celsius\",\"fahrenheit\"],\"description\":\"\"}},\"required\":[\"location\"]}}]";
            Assert.AreEqual(expected, json);
        }

        internal static string GetDLLPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var codebase = asm.CodeBase;
            UriBuilder uri = new UriBuilder(codebase);
            string path = Uri.UnescapeDataString(uri.Path);

            return path;
        }

        [TestMethod]
        public void DLLToolSetCanLoadFunctions()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: DLLToolSetCanLoadFunctions");

            var dllpath = GetDLLPath();
            var tool = new DLLFunctionTools(dllpath, "GenAIFramework.Test.Utilities");

            var functions = tool.GetFunctions();

            Assert.IsTrue(functions.Any());
        }

        [TestMethod]
        public async Task DLLToolSetCanExecuteFunction()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Test, "DLLToolSetCanExecuteFunction");

            var dllpath = GetDLLPath();
            var toolset = new DLLFunctionTools(dllpath, "GenAIFramework.Test.Utilities");

            var functions = toolset.GetFunctions();

            Assert.IsTrue(functions.Any());

            var tool = toolset.GetTool("PrintFinancialForecast");
            Assert.IsNotNull(tool);

            var context = new ExecutionContext();
            context["printer"] = "OfficePrinter";

            var result = await toolset.ExecuteAsync("PrintFinancialForecast", context);

            Assert.AreEqual("Printed the forecast to OfficePrinter", result);

            context["printer"] = "HomePrinter";
            result = await tool.ExecuteAsync(context);

            Assert.AreEqual("Printed the forecast to HomePrinter", result);

            object output = string.Empty;

            Assert.IsTrue(context.TryGetResult(tool.Name, out output));
            Assert.AreEqual("Printed the forecast to HomePrinter", output);
        }
    }
}
