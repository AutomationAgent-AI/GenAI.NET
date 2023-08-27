using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Services;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI
{
    /// <summary>
    /// Generative AI application class that instantiates GenerativeAIService.
    /// </summary>
    public class Application
    {
        private static GenerativeAIService aiservice;
        private static TextProviderService txtservice;
        private static ILanguageModel languageModel;

        private static ILanguageModel LanguageModel
        {
            get
            {
                if (null == languageModel)
                {
                    languageModel = DefaultLanguageModel;
                }

                return languageModel;
            }
        }

        /// <summary>
        /// Provides an instance of the default OpenAI language model
        /// </summary>
        public static ILanguageModel DefaultLanguageModel => new OpenAILanguageModel("gpt-3.5-turbo");

        //To be used for testing
        internal static void SetLanguageModel(ILanguageModel llm)
        {
            languageModel = llm;
        }

        /// <summary>
        /// Returns the generative AI service interface
        /// </summary>
        /// <returns></returns>
        public static IGenerativeAIService GetAIService()
        {
            if (null == aiservice) { aiservice = new GenerativeAIService(); }
            return aiservice;
        }

        internal static TextProviderService GetTextProviderService()
        {
            if(null == txtservice)
            {
                txtservice = new TextProviderService();
            }
            return txtservice;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Application() 
        {
        }

        /// <summary>
        /// Application must be initialized before making any request.
        /// </summary>
        /// <param name="llmtype">Large Language Model type such as OpenAI</param>
        /// <param name="model">Model name</param>
        /// <param name="apikey">apikey for the language model</param>
        /// <param name="logFilePath">Full path for log files.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Initialize(string llmtype, string model, string apikey, string logFilePath)
        {
            Logger.SetLogFile(logFilePath);
            var svc = GetAIService();
            switch (llmtype.ToLower())
            {
                case "openai":
                    languageModel = svc.CreateOpenAIModel(model, apikey);
                    break;
                default:
                    throw new NotImplementedException(llmtype);
            }
        }

        /// <summary>
        /// Sets Log file path
        /// </summary>
        /// <param name="logFilePath">Full path of the log file</param>
        public static void SetLogFilePath(string logFilePath)
        {
            Logger.SetLogFile(logFilePath);
        }

        /// <summary>
        /// Gets AI assitant's response for a given message.
        /// </summary>
        /// <param name="message">User's message</param>
        /// <param name="temperature">A value between 0 to 2, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>Response returned by the AI assitant.</returns>
        public static async Task<string> GetResponseAsync(string message, double temperature)
        {
            var svc = GetAIService();
            var chat = svc.CreateConversation("tempsession", LanguageModel);
            chat.AppendMessage(message, Role.user);

            var msg = await chat.GetResponseAsync(temperature);
            return msg.content;
        }

        internal static IEnumerable<string> GetFiles(string source, string[] extensions)
        {
            IEnumerable<string> files = Enumerable.Empty<string>();
            var directory = string.Empty;
            if (source.Contains("*"))
            {
                var searchpattern = Path.GetFileName(source);
                directory = Path.GetDirectoryName(source);
                files = Directory.GetFiles(directory, searchpattern, SearchOption.AllDirectories);
            }
            else if (Directory.Exists(source))
            {
                files = Directory.GetFiles(source);
            }
            else
            {
                files = Enumerable.Repeat(source, 1);
            }

            return files.Where(f => extensions.Contains(Path.GetExtension(f).ToLower()));
        }

        /// <summary>
        /// Splits the texts from the given source into smaller chunks based on chunkSize parameter and 
        /// serializes them to a vector database for semantic search.
        /// </summary>
        /// <param name="source">Source of information, it could be full path of a folder 
        /// or file. If the full path of a folder is provided, it will collect all the 
        /// supported files from the folder.</param>
        /// <param name="destination">Full path where the vector database needs to be saved.</param>
        /// <param name="chunkSize">Max number of characters of the chunk to be used by text splitter.</param>
        /// <param name="overlap">Max number of character overlap between the two consecutive chunks.</param>
        /// <returns>The vector store where data is saved</returns>
        /// <exception cref="System.Exception"></exception>
        public static IVectorStore SaveForSemanticSearch(string source, string destination, int chunkSize = 1000, int overlap = 100)
        {
            var svc = GetAIService();

            var txtservice = GetTextProviderService();
            if (txtservice == null) throw new System.Exception("TextProviderService not found!!");

            var extensions = new[] { ".txt", ".csv", ".pdf" };
            var files = GetFiles(source, extensions);

            List<ITextObject> textObjects = new List<ITextObject>();

            var splitter = TextSplitter.WithParameters(chunkSize, overlap);

            foreach (var file in files)
            {
                var texts = txtservice.EnumerateText(file, "English");
                foreach (var text in texts)
                {
                    var splits = splitter.Split(text);
                    textObjects.AddRange(splits);
                }
            }

            var transformer = svc.CreateVectorTransformer(TransformerType.OpenAIEmbedding);
            var store = svc.CreateVectorStore(transformer);
            store.Add(textObjects, true);
            store.Save(destination);

            return store;
        }

        /// <summary>
        /// Gets response from AI assitant using the context vector database.
        /// </summary>
        /// <param name="sessionid">Session id or the conversation id</param>
        /// <param name="systemctx">System message to specify, how to process a given message.</param>
        /// <param name="vdbpath">Full path of vector database to get the context.</param>
        /// <param name="message">User message for which response is required.</param>
        /// <param name="temperature">A value between 0 and 2 to control the randomness of the response.</param>
        /// <returns>Returns the AI assitant's response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static async Task<string> GetResponseFromContextAsync(string sessionid, string systemctx, string vdbpath, string message, double temperature) 
        {
            var svc = GetAIService();

            var chat = svc.GetConversation(sessionid);
            if(chat == null)
            {
                chat = svc.CreateConversation(sessionid, LanguageModel);
            }

            if(!string.IsNullOrEmpty(systemctx))
            {
                chat.AppendMessage(systemctx, Role.system);
            }

            if (File.Exists(vdbpath))
            {
                var store = svc.DeserializeVectorStore(vdbpath);
                if (store == null) throw new System.Exception($"Invalid vector store, {vdbpath}!!");
                chat.AddContext(store);
            }
            
            chat.AppendMessage(message, Role.user);

            var msg = await chat.GetResponseAsync(temperature);
            return msg.content;
        }
    }
}
