﻿using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    class SemanticSearch : SearchTool
    {
        private IVectorStore database;
        private Func<IVectorStore> dbFactory;
        private int chunkSize = 1000;
        private int chunkOverlap = 100;

        private SemanticSearch()
        {
            Name = "SemanticSearchTool";
            Description = "Performs a semantic search for a given query in its database and returns the relevant text chunks along with refernces.";
        }
        public SemanticSearch(IVectorStore store) : this()
        { 
            database = store;
        }

        public SemanticSearch(Func<IVectorStore> factory, int chunkSize = 1000, int chunkOverlap = 100) : this()
        {
            this.chunkSize = chunkSize;
            this.chunkOverlap = chunkOverlap;
            dbFactory = factory;
        }

        public async override Task<IEnumerable<SearchResult>> SearchAsync(string query, string context)
        {
            if(null == database)
            {
                database = await Task.Run(() => dbFactory.Invoke());
            }

            if (!string.IsNullOrEmpty(context))
            {
                UpdateStore(database, context, chunkSize, chunkOverlap);
            }

            return await Task.Run(() => {
                var matches = database.Search(TextObject.Create("query", query), count);

                return matches.Select(m => new SearchResult { content = m.Attributes["Text"], reference = m.Attributes["Name"] });
            });
        }

        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var result = await base.ExecuteCoreAsync(context);
            context["database"] = database;
            return result;
        }

        private static IVectorStore UpdateStore(IVectorStore store, 
            string source,
            int chunkSize = 1000,
            int chunkOverlap = 100)
        {
            var textObjects = TextExtractorTool.ExtractTextObjects(source);
            var splitter = TextSplitter.WithParameters(chunkSize, chunkOverlap);

            var splitTexts = new List<ITextObject>();
            foreach (var txt in textObjects)
            {
                var splits = splitter.Split(txt);
                splitTexts.AddRange(splits);
            }

            store.Add(splitTexts, true);

            return store;
        }
    }
}
