using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    class BingSearch : SearchTool
    {
        private readonly HttpTool httpTool;

        private BingSearch(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                apiKey = Configuration.Instance.BingAPIConfig.ApiKey;
            }

            var headers = new Dictionary<string, string>()
            {
                {"Ocp-Apim-Subscription-Key", apiKey }
            };

            httpTool = HttpTool.WithClient().WithDefaultRequestHeaders(headers);
        }

        /// <summary>
        /// Creates BingSearch tool with parameters
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="count">max search results to be sent</param>
        /// <returns>BingSearch objectS</returns>
        public static BingSearch WithApiKey(string apiKey, int count)
        {
            return new BingSearch(apiKey) { 
                count = count, 
                Name = "BingSearch", 
                Description="Performs web search using Bing and gets the URL and snippet of related articles on the web." 
            };
        }

        /// <summary>
        /// Performs Bing Search asynchronously
        /// </summary>
        /// <param name="query">Search query</param>
        /// <returns>List of search results</returns>
        public async override Task<IEnumerable<SearchResult>> SearchAsync(string query, string context)
        {
            string uri = $"{Configuration.Instance.BingAPIConfig.EndPointUrl}search?q={Uri.EscapeDataString(query)}&count={count+1}&offset=0";

            var json = await httpTool.GetAsync(uri);
            var data = Deserialize<BingSearchResult>(json);
            if (data != null && data.webPages != null && data.webPages.value != null)
            {
                List<SearchResult> results = new List<SearchResult>();
                foreach (var item in data.webPages.value)
                {
                    //var content = await GetRelevantContent(item.snippet, item.url);
                    results.Add(new SearchResult { content = item.snippet, reference = item.url });
                }

                return results;
            }

            return Enumerable.Empty<SearchResult>();
        }

        //private async Task<string> GetRelevantContent(string content, string url)
        //{
        //    //return await Task.FromResult(content);
            
        //    var text = WebContentExtractor.GetTextContentFromWebpage(url);
        //    if (text.Length < 500) return text;

        //    var tool = ForSemanticSearchFromSource(text, 1000, 0).WithMaxResultCount(2);
            
        //    var results = await tool.SearchAsync(content);

        //    return string.Join(Environment.NewLine, results.Select(x => x.content));
        //}
    }

    class BingSearchResult
    {
        public WebPages webPages { get; set; }
    }

    class WebPages
    {
        public WebPage[] value { get; set; }
    }

    class WebPage
    {
        public string url { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string snippet { get; set; } = string.Empty;
    }
}
