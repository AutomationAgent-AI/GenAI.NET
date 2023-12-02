using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI
{
    /// <summary>
    /// Implements the configuration for OpenAI/AzureOpenAI
    /// </summary>
    internal class OpenAIConfig
    {
        /// <summary>
        /// Endpoint URL which contains resource name for azure
        /// </summary>
        public string EndPointUrl { get; set; }

        /// <summary>
        /// Deployment ID for GPT Model in Azure
        /// </summary>
        public string GPTDeployment { get; set; }

        /// <summary>
        /// Deployment ID for Text Embedding model in Azure
        /// </summary>
        public string EmbeddingDeployment { get; set; }

        /// <summary>
        /// ApiVersion, applicable to Azure
        /// </summary>
        public string ApiVersion { get; set; }

        private string apiKey; //stores api key
        
        /// <summary>
        /// API Key
        /// </summary>
        public string ApiKey 
        {
            get
            {
                if (string.IsNullOrEmpty(apiKey))
                {
                    if (AzureConfig)
                    {
                        apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
                    }
                    else
                    {
                        apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                    }
                }
                return apiKey;
            }
            set
            {
                apiKey = value;
            } 
        }

        /// <summary>
        /// GPT Model Name
        /// </summary>
        public string Model { get; set; } = "gpt-3.5-turbo"; //default model for OpenAI

        /// <summary>
        /// Provides token limit for the given model
        /// </summary>
        public int TokenLimit { get; set; } = 4000; //Default is 4K model
        
        /// <summary>
        /// Gets full URL for the Text Embedding API
        /// </summary>
        public string EmbeddingUrl 
        { 
            get 
            {
                if(AzureConfig)
                {
                    if (string.IsNullOrEmpty(EmbeddingDeployment) || string.IsNullOrEmpty(ApiVersion)) return string.Empty;

                    return $"{EndPointUrl}openai/deployments/{EmbeddingDeployment}/embeddings?api-version={ApiVersion}";
                }
                return "https://api.openai.com/v1/embeddings";
            } 
        }

        /// <summary>
        /// Gets full URL for the chat completion API
        /// </summary>
        public string CompletionsUrl
        {
            get
            {
                if (AzureConfig)
                {
                    if (string.IsNullOrEmpty(GPTDeployment) || string.IsNullOrEmpty(ApiVersion)) return string.Empty;

                    return $"{EndPointUrl}openai/deployments/{GPTDeployment}/chat/completions?api-version={ApiVersion}";
                }
                return "https://api.openai.com/v1/chat/completions";
            }
        }

        /// <summary>
        /// Checks if it is Azure config
        /// </summary>
        public bool AzureConfig
        {
            get
            {
                if(string.IsNullOrEmpty(EndPointUrl) || EndPointUrl.Contains("api.openai.com")) 
                { 
                    return false; 
                }
                return !string.IsNullOrEmpty(GPTDeployment) && !string.IsNullOrEmpty(ApiVersion);
            }
        }
    }

    /// <summary>
    /// Implements config for Bing Search API
    /// </summary>
    internal class BingAPIConfig
    {
        /// <summary>
        /// Bing API endpoint url
        /// </summary>
        public string EndPointUrl { get; set; } = "https://api.bing.microsoft.com/v7.0/";

        private string apiKey; //stores api key

        /// <summary>
        /// Api Key
        /// </summary>
        public string ApiKey
        {
            get
            {
                if (string.IsNullOrEmpty(apiKey))
                {
                    apiKey = Environment.GetEnvironmentVariable("BING_API_KEY");
                }
                return apiKey;
            }
            set
            {
                apiKey = value;
            }
        }
    }

    /// <summary>
    /// Implements a configuration
    /// </summary>
    internal class Configuration
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public Configuration() { }
        private static Configuration instance = null;

        /// <summary>
        /// Global instance of the configuration
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if(instance == null)
                {
                    var exedir = Path.GetDirectoryName(GetDLLPath());
                    var config = Path.Combine(exedir, "config.json");
                    instance = Load(config);
                }
                return instance;
            }
        }

        /// <summary>
        /// Configuration for OpenAI or AzureOpenAI APIs
        /// </summary>
        public OpenAIConfig OpenAIConfig { get; set; } = new OpenAIConfig();

        /// <summary>
        /// Configuration for Bing API
        /// </summary>
        public BingAPIConfig BingAPIConfig { get; set; } = new BingAPIConfig();

        /// <summary>
        /// Log file path
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Gets executing DLL Path
        /// </summary>
        /// <returns></returns>
        internal static string GetDLLPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var codebase = asm.CodeBase;
            UriBuilder uri = new UriBuilder(codebase);
            string path = Uri.UnescapeDataString(uri.Path);

            return path;
        }

        /// <summary>
        /// Saves the config as json to a given file
        /// </summary>
        /// <param name="filePath">Full file path to save the config as json</param>
        public void Save(string filePath)
        {
            var jsonfile = Path.ChangeExtension(filePath, "json");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsontxt = serializer.Serialize(this);
            File.WriteAllText(jsonfile, jsontxt);
        }

        /// <summary>
        /// Loads the configuration from the given json file
        /// </summary>
        /// <param name="filePath">Full path of config json file</param>
        /// <returns>Configuration</returns>
        public static Configuration Load(string filePath)
        {
            if(!File.Exists(filePath)) return new Configuration();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsontxt = File.ReadAllText(filePath);
            return serializer.Deserialize<Configuration>(jsontxt);
        }
    }
}
