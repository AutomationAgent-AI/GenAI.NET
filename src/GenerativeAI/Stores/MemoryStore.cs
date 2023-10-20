using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Stores
{
    /// <summary>
    /// Represents long term memory store
    /// </summary>
    public class MemoryStore : IMemoryStore
    {
        private IVectorStore store = null;
        private readonly List<ChatMessage> history = new List<ChatMessage>();
        private int maxCharacters = 1000;
        
        class Data
        {
            public List<ChatMessage> history { get; set; }
            public int maxCharacters { get; set; }
            public string vectorStorePath { get; set; }
            public bool SemanticSearch { get; set; }
        }

        /// <summary>
        /// Configures the memory store
        /// </summary>
        /// <param name="maxCharacters">Max characters allowed for chat history</param>
        /// <param name="vectorStore">Vector store to be used for semantic search</param>
        public void Configure(int maxCharacters, IVectorStore vectorStore)
        {
            this.maxCharacters = maxCharacters;
            this.store = vectorStore;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(ChatMessage message)
        {
            history.Add(message);
            if (store != null)
            {
                store.Add(new ITextObject[] { TextObject.Create($"MSG{history.Count}", message.content, message.role) }, true);
            }
        }

        /// <summary>
        /// Gets the chat history
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IEnumerable<ChatMessage> ChatHistory(string query)
        {
            int len = 0;
            if (store != null && !string.IsNullOrWhiteSpace(query))
            {
                var matches = store.Search(TextObject.Create("MSG", query), history.Count);
                Dictionary<int, ChatMessage> messages = new Dictionary<int, ChatMessage>();
                foreach (var match in matches)
                {
                    var name = match.Attributes["Name"];
                    var role = match.Attributes["Class"];
                    var content = match.Attributes["Text"];
                    len += content.Length;
                    if (len > maxCharacters) break;
                    var id = int.Parse(name.Substring(3));
                    messages.Add(id, new ChatMessage((Role)Enum.Parse(typeof(Role), role), content));
                }
                return messages.OrderBy(p => p.Key).Select(p => p.Value);
            }
            
            return history.Select(x => x).Reverse().TakeWhile(m => (len += m.content.Length) < maxCharacters).Reverse();
        }

        /// <summary>
        /// Creates memory store from a Json file
        /// </summary>
        /// <param name="jsonFile">Full path of the json file</param>
        /// <returns></returns>
        public static MemoryStore FromJsonFile(string jsonFile)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var data = serializer.Deserialize<Data>(File.ReadAllText(jsonFile));
                var memory = new MemoryStore();
                memory.maxCharacters = data.maxCharacters;
                if(!string.IsNullOrEmpty(data.vectorStorePath) && File.Exists(data.vectorStorePath))
                {
                    var store = VectorStore.Create(data.vectorStorePath);
                    memory.store = store;
                }
                else if (data.SemanticSearch)
                {
                    memory.store = new VectorStore(new OpenAIEmbeddingTransformer());
                    data.history.ForEach(x => memory.AddMessage(x));
                }

                if(!memory.history.Any())
                {
                    memory.history.AddRange(data.history);
                }

                return memory;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.Message);
                Logger.WriteLog(LogLevel.StackTrace, LogOps.Exception, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Saves the memory to a given file path
        /// </summary>
        /// <param name="filepath"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Save(string filepath)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string vdbpath = string.Empty;

            if(store != null)
            {
                vdbpath = Path.ChangeExtension(filepath, "vdb");
                store.Save(vdbpath);
            }
            var data = new Data() { history = history, maxCharacters = maxCharacters, vectorStorePath = vdbpath, SemanticSearch = (store != null) };
            var txt = serializer.Serialize(data);
            File.WriteAllText(filepath, txt );
        }

        /// <summary>
        /// Clears the chat history
        /// </summary>
        public void Clear()
        {
            history.Clear();
        }
    }
}
