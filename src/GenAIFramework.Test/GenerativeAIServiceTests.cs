﻿using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GenerativeApp = Automation.GenerativeAI.Application;

namespace GenAIFramework.Test
{
    [TestClass]
    public class GenerativeAIServiceTests
    {
        private string RootPath = string.Empty;
        private ILanguageModel languageModel;

        public GenerativeAIServiceTests()
        {
            RootPath = Assembly.GetExecutingAssembly().Location;
            var logfile = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\GenerativeAIServiceTests.log");
            Logger.SetLogFile(logfile);

            var responses = new Dictionary<string, string>()
            {
                { "Hi, there!! I am Ram", "Hello Ram! How can I assist you today?"},
                { "What is my name and which fruit do I like?", "Your name is Romeo and you like to eat apples for breakfast."},
                { "What do I do for living?","You work as a Software Engineer." },
                { "Where is Timbaktu?", "Timbaktu is a village located in the Anantapur district of the state of Andhra Pradesh in southern India." },
                { "I live in Kolkata, Which Indian State do I live in?", "You live in the Indian state of West Bengal, with Kolkata being its capital city." },
                { "What is my name?", "Your name is Romeo." },
                { "What is the capital of the state I live in?", "The capital of the state you live in is Kolkata." },
                { "What is the revenue of Meta in Q1 of 2023?", "The revenue of Meta in Q1 of 2023 was $28.6 billion." },
                { "Provide me trend on revenue growth, expenses, share price and other financial data for Meta", "{\"revenue_grow_trend\": \"positive\"}" }
            };

            languageModel = new MockLanguageModel("Mock", responses);
            //languageModel = new OpenAILanguageModel("gpt-3.5-turbo");
            GenerativeApp.SetLanguageModel(languageModel);
        }

        [TestMethod]
        public void SaveVectorStore()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: SaveVectorStore");
            var service = GenerativeApp.GetAIService();
            Assert.IsNotNull(service);
            var transformer = service.CreateVectorTransformer(TransformerType.OpenAIEmbedding);
            Assert.IsNotNull(transformer);
            var store = service.CreateVectorStore(transformer);
            Assert.IsNotNull(store);

            var data = new[]
            {
                TextObject.Create("S1", "I am feeling good", "Positive"),
                TextObject.Create("S2", "It's a happy day", "Positive"),
                TextObject.Create("S3", "Sad day today", "Negative"),
                TextObject.Create("S4", "I am very happy today", "Positive"),
                TextObject.Create("S5", "This looks good", "Positive"),
                TextObject.Create("S6", "I am in a bad mood", "Negative"),
                TextObject.Create("S7", "Why are you so sad?", "Negative"),
                TextObject.Create("S8", "My wound is very bad", "Negative")
            };

            store.Add(data, true);

            var results = store.Search(TextObject.Create("S0", "I feel good"), 2);
            Assert.IsTrue(results.Count() == 2);

            Assert.AreEqual("S1", results.First().Attributes["Name"]);

            var dbpath = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\VectStore.vdb");
            if (File.Exists(dbpath)) File.Delete(dbpath);

            store.Save(dbpath);

            Assert.IsTrue(File.Exists(dbpath));
        }

        [TestMethod]
        public void CreateVectorStore()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: CreateVectorStore");
            var service = GenerativeApp.GetAIService();
            Assert.IsNotNull(service);

            var dbpath = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\VectStore.vdb");
            var store = service.DeserializeVectorStore(dbpath);

            Assert.IsNotNull(store);
            var results = store.Search(TextObject.Create("S0", "I feel good"), 2);
            Assert.IsTrue(results.Count() == 2);

            Assert.AreEqual("S1", results.First().Attributes["Name"]);
        }

        [TestMethod]
        public void OpenAIEmbedding()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: OpenAIEmbedding");
            var service = GenerativeApp.GetAIService();
            Assert.IsNotNull(service);

            var transformer = service.CreateVectorTransformer(TransformerType.OpenAIEmbedding);
            Assert.IsNotNull(transformer);

            var store = service.CreateVectorStore(transformer);
            Assert.IsNotNull(store);

            var data = new[]
            {
                TextObject.Create("S1", "나는 오늘 매우 행복하고 기분이 좋습니다", "Korean"),
                TextObject.Create("S2", "I am very happy today and feeling wonderful", "English"),
            };

            store.Add(data, true);
            var results = store.Search(TextObject.Create("S0", "It's a wonderful feeling that I am happy today"), 2).ToArray();
            Assert.IsTrue(results.Length == 2);

            Assert.IsTrue(results[0].Score > 0.95);
            Assert.IsTrue(results[1].Score > 0.85);

            results = store.Search(TextObject.Create("S0", "I feel disgusted becuase I am sad!"), 2).ToArray();
            Assert.IsTrue(results.Length == 2);
            Assert.IsTrue(results[0].Score < 0.8);
            Assert.IsTrue(results[1].Score < 0.8);
        }

        [TestMethod]
        public async Task HelloWorldChat()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: HelloWorldChat");
            var service = GenerativeApp.GetAIService();
            Assert.IsNotNull(service);

            var chat = service.CreateConversation("hello", languageModel);
            Assert.IsNotNull(chat);
            Assert.AreEqual("hello", chat.Id);

            chat.AppendMessage("Hi, there!! I am Ram", Role.user);

            var response = await chat.GetResponseAsync(0.8);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.content.Contains("Ram"));
        }

        [TestMethod]
        public async Task ContextBasedQuestions()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: ContextBasedQuestions");
            var service = GenerativeApp.GetAIService();
            Assert.IsNotNull(service);

            var chat = service.CreateConversation("context", languageModel);

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

        [TestMethod]
        public async Task GetResponse()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: GetResponse");
            var response = await GenerativeApp.GetResponseAsync("Where is Timbaktu?", 0);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Anantapur"));
            Assert.IsTrue(response.Contains("Andhra"));
        }

        [TestMethod]
        public void SaveForSemanticSearch()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: SaveForSemanticSearch");
            var source = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.txt");
            var destination = Path.Combine(RootPath, @"..\..\..\..\..\tests\output\article.vdb");
            if(File.Exists(destination)) { File.Delete(destination); }

            var result = GenerativeApp.CreateVectorDatabaseForSemanticSearch(source, destination);

            Assert.IsNotNull(result);
            Assert.IsTrue(File.Exists(destination));
        }

        [TestMethod]
        public async Task GetResponseFromContextWithoutVDB() 
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: GetResponseFromContextWithoutVDB");
            var sessionid = "GetResponseFromContextWithoutVDB";
            var msg = "Hi, My name is Romeo. I live in Kolkata. I work as a Software Engineer. I love to eat apple for breakfast. Which Indian State do I live in?";
            var response = await GenerativeApp.GetResponseFromContextAsync(sessionid, string.Empty, string.Empty, msg, 0);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("West Bengal"));
            
            response = await GenerativeApp.GetResponseFromContextAsync(sessionid, string.Empty, string.Empty, "What is my name?", 0.2);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Romeo"));

            response = await GenerativeApp.GetResponseFromContextAsync(sessionid, string.Empty, string.Empty, "What is the capital of the state I live in?", 0.2);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("Kolkata"));
        }

        [TestMethod]
        public async Task GetResponseFromContextWithVDB()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: GetResponseFromContextWithVDB");
            var sessionid = "GetResponseFromContextWithVDB";
            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var msg = "What is the revenue of Meta in Q1 of 2023?";

            var response = await GenerativeApp.GetResponseFromContextAsync(sessionid, string.Empty, vdb, msg, 0);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("28.6"));
        }

        [TestMethod]
        public async Task GetResponseFromContextWithSystemCtx()
        {
            Logger.WriteLog(LogLevel.Info, LogOps.Command, "Test: GetResponseFromContextWithSystemCtx");
            var sessionid = "GetResponseFromContextWithVDB";
            var vdb = Path.Combine(RootPath, @"..\..\..\..\..\tests\input\article.vdb");

            var msg = "Provide me trend on revenue growth, expenses, share price and other financial data for Meta";
            var sysctx = "Act like a programmer who always classifies trends as positive or negative, financial values as numbers and returns the response in JSON format.";

            var response = await GenerativeApp.GetResponseFromContextAsync(sessionid, sysctx, vdb, msg, 0.2);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Contains("positive"));
            Assert.IsTrue(response.Contains("{") && response.Contains("}")); //check for JSON format.
        }
    }
}
