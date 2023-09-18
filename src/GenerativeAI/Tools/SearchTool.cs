using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// SearchResult as returned by the SearchTool
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Content snippet
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// Reference URL or document name
        /// </summary>
        public string reference { get; set; }

        /// <summary>
        /// Override Equals method to just compare references.
        /// </summary>
        /// <param name="obj">Other object to be compared with.</param>
        /// <returns>True if both objects are same.</returns>
        public override bool Equals(object obj)
        {
            var result = obj as SearchResult;
            if (result == null) return false;
            
            return string.Equals(reference, result.reference);
        }

        /// <summary>
        /// Overrides the GetHashCode method to return hash code based on reference.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return reference.GetHashCode();
        }
    }

    /// <summary>
    /// SearchTool implementation
    /// </summary>
    public abstract class SearchTool : FunctionTool
    {
        /// <summary>
        /// Max result count expected out of search
        /// </summary>
        protected int count = 5;

        /// <summary>
        /// The query parameter description for the search tool.
        /// </summary>
        public static readonly ParameterDescriptor QueryParameter = new ParameterDescriptor() { Name="query", Description="Text or query string to search.", Type=TypeDescriptor.StringType };
        
        /// <summary>
        /// The context parameter for the search tool.
        /// </summary>
        public static readonly ParameterDescriptor ContextParameter = new ParameterDescriptor() { Name = "context", Description = "Context to search from", Required = false, Type = TypeDescriptor.StringType };

        /// <summary>
        /// Performs search
        /// </summary>
        /// <param name="query">What to search</param>
        /// <param name="context">Optional parameter, Context from where to search</param>
        /// <returns>Search results</returns>
        public abstract Task<IEnumerable<SearchResult>> SearchAsync(string query, string context);

        /// <summary>
        /// Creates BingSearch tool with parameters
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <returns>SearchTool for Bing Search</returns>
        public static SearchTool ForBingSearch(string apiKey)
        {
            return BingSearch.WithApiKey(apiKey, 5);
        }

        /// <summary>
        /// Creates a search tool using a vector store for semantic search.
        /// </summary>
        /// <param name="store">Vector store to be used for semantic search</param>
        /// <returns>SearchTool for semantic search</returns>
        public static SearchTool ForSemanticeSearch(IVectorStore store)
        {
            return new SemanticSearch(store);
        }

        /// <summary>
        /// Creates a search tool for semantic search using a given vector database
        /// </summary>
        /// <param name="dbPath">Full path of the vector database</param>
        /// <returns>SearchTool</returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static SearchTool ForSemanticSearchFromDatabase(string dbPath)
        {
            if (!File.Exists(dbPath)) throw new FileNotFoundException(dbPath);

            Func<string, IVectorStore> factory = (ctx) =>
            {
                var service = Application.GetAIService();

                return service.DeserializeVectorStore(dbPath);
            };

            return new SemanticSearch(factory);
        }

        /// <summary>
        /// Creates a search tool for semantic search using a given source, such as 
        /// Document or Text content. If source is a document, then it needs full path
        /// for the document including wildcard path. This tool uses a pipeline of TextExtractor,
        /// TextSplitter and Semantic Search tools.
        /// </summary>
        /// <param name="source">Plain text or full path of document to be used for semantic search.</param>
        /// <param name="chunkSize">Max number of characters in the chunk to be used by text splitter</param>
        /// <param name="chunkOverlap">Max number of character overlap between the two consecutive chunks.</param>
        /// <param name="transformerType">Type of transformer to be used for indexing</param>
        /// <returns>SearchTool for semantic search</returns>
        public static SearchTool ForSemanticSearchFromSource(
            string source, 
            int chunkSize = 1000, 
            int chunkOverlap = 100, 
            TransformerType transformerType = TransformerType.OpenAIEmbedding)
        {
            Func<string, IVectorStore> factory = (ctx) => CreateVectorStore(source, chunkSize, chunkOverlap, transformerType);

            return new SemanticSearch(factory);
        }

        private static IVectorStore CreateVectorStore(string source,
            int chunkSize = 1000,
            int chunkOverlap = 100,
            TransformerType transformerType = TransformerType.OpenAIEmbedding)
        {
            var textObjects = TextExtractorTool.ExtractTextObjects(source);
            var splitter = TextSplitter.WithParameters(chunkSize, chunkOverlap);

            var splitTexts = new List<ITextObject>();
            foreach (var txt in textObjects)
            {
                var splits = splitter.Split(txt);
                splitTexts.AddRange(splits);
            }

            var service = Application.GetAIService();

            var transformer = service.CreateVectorTransformer(transformerType);
            var store = service.CreateVectorStore(transformer);

            store.Add(splitTexts, true);

            return store;
        }

        /// <summary>
        /// Sets max result count parameter to the tool. The default value is 5.
        /// </summary>
        /// <param name="count">Count of maximum results to be expected by this search tool.</param>
        /// <returns>SearchTool</returns>
        public SearchTool WithMaxResultCount(int count) { this.count = count; return this; }

        /// <summary>
        /// Overrides the core implementation logic to execute the search tool.
        /// </summary>
        /// <param name="context">ExecutionContext</param>
        /// <returns>Execution Result</returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var result = new Result();
            object query = string.Empty;
            if (!context.TryGetValue(QueryParameter.Name, out query)) return result;
            
            object ctx = string.Empty;
            context.TryGetValue(ContextParameter.Name, out ctx); //optional parameter

            var results = await SearchAsync((string)query, (string)ctx);
            if(results != null && results.Any())
            {
                result.success = true;
                result.output = results;
            }

            return result;
        }

        /// <summary>
        /// Provides function descriptor for this search tool.
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            return new FunctionDescriptor(Name, Description, new List<ParameterDescriptor>() { QueryParameter, ContextParameter });
        }
    }
}
