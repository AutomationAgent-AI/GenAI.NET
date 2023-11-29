using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Stores
{
    /// <summary>
	/// Represents an embedding result returned by the Embedding API.  
	/// </summary>
	class EmbeddingResult
    {
        /// <summary>
        /// List of results of the embedding
        /// </summary>
        public List<Data> data { get; set; }

        /// <summary>
        /// Allows an EmbeddingResult to be implicitly cast to the array of floats repsresenting the first ebmedding result
        /// </summary>
        /// <param name="embeddingResult">The <see cref="EmbeddingResult"/> to cast to an array of floats.</param>
        public static implicit operator double[](EmbeddingResult embeddingResult)
        {
            return embeddingResult.data.FirstOrDefault()?.embedding;
        }
    }

    /// <summary>
    /// Data returned from the Embedding API.
    /// </summary>
    class Data
    {
        /// <summary>
        /// The input text represented as a vector (list) of floating point numbers
        /// </summary>
        public double[] embedding { get; set; }

        /// <summary>
        /// Index
        /// </summary>
        public int index { get; set; }
    }

    class EmbeddingRequest
    {
        public string input { get; set; }
        public string model => "text-embedding-ada-002";
    }

    [Serializable]
    internal class OpenAIEmbeddingTransformer : IVectorTransformer
    {
        public int VectorLength => 1536;

        public OpenAIEmbeddingTransformer()
        {  
        }

        HttpTool HttpTool 
        {
            get
            {
                var config = Configuration.Instance.OpenAIConfig;
                var headers = new Dictionary<string, string>();
                if (config.AzureConfig)
                {
                    headers["api-key"] = config.ApiKey;
                }
                else
                {
                    headers["Authorization"] = $"Bearer {config.ApiKey}";
                }

                return HttpTool.WithClient().WithDefaultRequestHeaders(headers);
            }
        }

        public double[] Transform(string textObject)
        {
            var data = new EmbeddingRequest() { input = textObject };
            var serializer = new JavaScriptSerializer();

            string jsonPayload = serializer.Serialize(data);
            try
            {
                string json = this.HttpTool.PostAsync(Configuration.Instance.OpenAIConfig.EmbeddingUrl, jsonPayload).GetAwaiter().GetResult();
                var result = serializer.Deserialize<EmbeddingResult>(json);
                return (double[])result;
            }
            catch (WebException ex)
            {
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.Message);
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.StackTrace);
                return Enumerable.Empty<double>().ToArray();
            }
        }

        public double[][] Transform(IEnumerable<string> textObjects)
        {
            return textObjects.Select(x => Transform(x)).ToArray();
        }
    }
}
