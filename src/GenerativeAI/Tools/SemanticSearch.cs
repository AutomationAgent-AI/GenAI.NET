using Automation.GenerativeAI.Interfaces;
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

        private SemanticSearch()
        {
            Name = "SemanticSearchTool";
            Description = "Performs a semantic search for a given query in its database and returns the relevant text chunks along with refernces.";
        }
        public SemanticSearch(IVectorStore store) : this()
        { 
            database = store;
        }

        public SemanticSearch(Func<IVectorStore> factory) : this()
        { 
            dbFactory = factory;
        }

        public async override Task<IEnumerable<SearchResult>> SearchAsync(string query)
        {
            if(null == database)
            {
                database = await Task.Run(dbFactory);
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
    }
}
