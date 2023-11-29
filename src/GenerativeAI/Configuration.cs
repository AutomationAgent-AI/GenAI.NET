using System;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI
{
    internal class OpenAIConfig
    {
        public string EndPointUrl { get; set; }
        public string GPTDeployment { get; set; }
        public string EmbeddingDeployment { get; set; }
        public string ApiVersion { get; set; }
        private string apiKey;
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
        public string Model { get; set; }
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

    internal class BingAPIConfig
    {
        public string EndPointUrl { get; set; } = "https://api.bing.microsoft.com/v7.0/";

        private string apiKey;
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

    internal class Configuration
    {
        public Configuration() { }
        private static Configuration instance = null;

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

        public OpenAIConfig OpenAIConfig { get; set; } = new OpenAIConfig();

        public BingAPIConfig BingAPIConfig { get; set; } = new BingAPIConfig();

        public string LogFile { get; set; }

        internal static string GetDLLPath()
        {
            var asm = Assembly.GetExecutingAssembly();
            var codebase = asm.CodeBase;
            UriBuilder uri = new UriBuilder(codebase);
            string path = Uri.UnescapeDataString(uri.Path);

            return path;
        }

        public void Save(string filePath)
        {
            var jsonfile = Path.ChangeExtension(filePath, "json");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsontxt = serializer.Serialize(this);
            File.WriteAllText(jsonfile, jsontxt);
        }

        public static Configuration Load(string filePath)
        {
            if(!File.Exists(filePath)) return new Configuration();
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var jsontxt = File.ReadAllText(filePath);
            return serializer.Deserialize<Configuration>(jsontxt);
        }
    }
}
