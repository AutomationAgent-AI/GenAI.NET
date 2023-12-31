﻿using Automation.GenerativeAI.Agents;
using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.LLM;
using Automation.GenerativeAI.Services;
using Automation.GenerativeAI.Stores;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using ExecutionContext = Automation.GenerativeAI.Interfaces.ExecutionContext;

namespace Automation.GenerativeAI
{
    class ToolDescriptor : IFunctionTool
    {
        public ToolDescriptor(string name, string description, IEnumerable<ParameterDescriptor> parameters) 
        {
            Descriptor = new FunctionDescriptor(name, description, parameters);
        }

        public string Name => Descriptor.Name;

        public FunctionDescriptor Descriptor { get; set; }

        public async Task<string> ExecuteAsync(ExecutionContext context)
        {
            //No execution logic
            return await Task.FromResult(string.Empty);
        }
    }


    /// <summary>
    /// Generative AI application class that instantiates GenerativeAIService.
    /// </summary>
    public class Application
    {
        private static GenerativeAIService aiservice;
        private static TextProviderService txtservice;
        private static ILanguageModel languageModel;
        private static ToolsCollection toolsCollection = new ToolsCollection();
        private static ExecutionContext exeResults = new ExecutionContext();
        private static Dictionary<string, IMemoryStore> memories = new Dictionary<string, IMemoryStore>();
        private static Dictionary<string, Agent> agents = new Dictionary<string, Agent>();
        
        internal static void Reset()
        {
            toolsCollection = new ToolsCollection();
            agents.Clear();
            memories.Clear();
        }

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
        public static ILanguageModel DefaultLanguageModel => new OpenAIClient(Configuration.Instance.OpenAIConfig);

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
        public static void Initialize(string llmtype, string model, string apikey, string logFilePath)
        {
            Logger.SetLogFile(logFilePath);
            var svc = GetAIService();
            switch (llmtype.ToLower())
            {
                case "openai":
                    Configuration.Instance.OpenAIConfig = new OpenAIConfig() { ApiKey = apikey };
                    languageModel = svc.CreateOpenAIModel(model, apikey);
                    break;
                default:
                    throw new NotImplementedException(llmtype);
            }
        }

        /// <summary>
        /// Initializes AzureOpenAI language model. Application must be initialized before making any request.
        /// </summary>
        /// <param name="azureEndpoint">Endpoint URL for Azure OpenAI service</param>
        /// <param name="gptDeployment">Deployment Name for GPT model</param>
        /// <param name="embeddingDeployment">Deployment Name for text embedding model</param>
        /// <param name="apiversion">API version</param>
        /// <param name="apiKey">ApiKey for the language model</param>
        /// <param name="model">Model name to be used for chat completion</param>
        /// <param name="logFilePath">Full path for log file.</param>
        public static void InitializeAzureOpenAI(string azureEndpoint, string gptDeployment, string embeddingDeployment, string apiversion, string apiKey, string model, string logFilePath)
        {
            Configuration.Instance.OpenAIConfig = new OpenAIConfig()
            {
                EndPointUrl = azureEndpoint,
                GPTDeployment = gptDeployment,
                EmbeddingDeployment = embeddingDeployment,
                ApiVersion = apiversion,
                ApiKey = apiKey,
                Model = model
            };
            Configuration.Instance.LogFile = logFilePath;
            Logger.SetLogFile(logFilePath);
            languageModel = new AzureOpenAILanguageModel(Configuration.Instance.OpenAIConfig);
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
        /// Gets AI assitant's response for a given message. This request doesn't have any memory.
        /// </summary>
        /// <param name="message">User's message</param>
        /// <param name="temperature">A value between 0 to 1, that controls randomness of the response. 
        /// Higher temperature will lead to more randomness. Lower temperature will be more deterministic.</param>
        /// <returns>Response returned by the AI assitant.</returns>
        public static async Task<string> GetResponseAsync(string message, double temperature)
        {
            var chatmsg = new ChatMessage(Role.user, message);

            LLMResponse response;

            if (toolsCollection.Any())
            {
                var systemmsg = Conversation.GetFunctionCallSystemMessage();
                response = await LanguageModel.GetResponseAsync(new[] { systemmsg, chatmsg }, toolsCollection.GetFunctions(), temperature);
            }
            else
            {
                response = await LanguageModel.GetResponseAsync(new[] { chatmsg }, temperature);
            }

            var msg = Conversation.MessgeFromResponse(response);
            if (msg == null) return $"ERROR: Failed to get response from the language model!!";

            if(msg is FunctionCallMessage)
            {
                return FunctionTool.ToJsonString(msg);
            }
            return msg.content;
        }

        /// <summary>
        /// Sends message to language model to get AI assistant's response.
        /// </summary>
        /// <param name="message">User's message</param>
        /// <param name="temperature">A value between 0 and 1 to control the randomness of the response.</param>
        /// <returns>Response message</returns>
        public static string GetResponse(string message, double temperature)
        {
            return GetResponseAsync(message, temperature).GetAwaiter().GetResult();
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
            else if (File.Exists(source))
            {
                files = Enumerable.Repeat(source, 1);
            }
            else
            {
                files = Enumerable.Empty<string>();
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
        public static IVectorStore CreateVectorDatabaseForSemanticSearch(string source, string destination, int chunkSize = 1000, int overlap = 100)
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
        /// <param name="temperature">A value between 0 and 1 to control the randomness of the response.</param>
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

            if (toolsCollection.Any())
            {
                chat.AddToolSet(toolsCollection);
            }
            
            chat.AppendMessage(message, Role.user);

            var msg = await chat.GetResponseAsync(temperature);
            if (msg is FunctionCallMessage)
            {
                return FunctionTool.ToJsonString(msg);
            }
            return msg.content;
        }

        /// <summary>
        /// Gets response from AI assitant using the context vector database.
        /// </summary>
        /// <param name="sessionid">Session id or the conversation id</param>
        /// <param name="systemctx">System message to specify, how to process a given message.</param>
        /// <param name="vdbpath">Full path of vector database to get the context.</param>
        /// <param name="message">User message for which response is required.</param>
        /// <param name="temperature">A value between 0 and 1 to control the randomness of the response.</param>
        /// <returns>Returns the AI assitant's response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string GetResponseFromContext(string sessionid, string systemctx, string vdbpath, string message, double temperature)
        {
            return GetResponseFromContextAsync(sessionid, systemctx, vdbpath, message, temperature).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a tool descriptor that can be passed to the language model or Agent as a tool.
        /// This tool doesn't have any execution logic and when required client can perform custom
        /// logic for its execution.
        /// </summary>
        /// <param name="name">Name of the tool</param>
        /// <param name="description">Description of the tool</param>
        /// <param name="parametersTableCSVAsText">A table to contain parameter name, description and type</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateToolDescriptor(string name, string description, string parametersTableCSVAsText)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            List<ParameterDescriptor> parameterDescriptors = new List<ParameterDescriptor>();

            var lines = parametersTableCSVAsText.Split('\n');

            var indices = new Dictionary<string, int>() { { "name", 0 }, { "description", 1 }, { "type", 2 } };
            int rowindex = 0;
            foreach(var line in lines)
            {
                var props = line.Split(',').Select(s => s.Trim()).ToArray();
                if (rowindex == 0 && props.Any(x => string.Equals(x, "description", StringComparison.InvariantCultureIgnoreCase)))
                {
                    //Header row
                    int j = 0;
                    foreach (var prop in props)
                    {
                        indices[prop.ToLower()] = j++;
                    }

                    continue;
                }

                var descriptor = new ParameterDescriptor() { Name = props[indices["name"]], Description = props[indices["description"]] };
                
                if(props.Length > 2)
                {
                    string type = props[indices["type"]];
                    switch (type)
                    {
                        case "int":
                        case "integer":
                            descriptor.Type = TypeDescriptor.IntegerType; break;
                        case "float":
                        case "double":
                        case "number":
                            descriptor.Type = TypeDescriptor.NumberType; break;
                        case "boolean":
                            descriptor.Type = TypeDescriptor.BooleanType; break;
                        case "string":
                            descriptor.Type = TypeDescriptor.StringType; break;
                        default:
                            break;
                    }
                }
                parameterDescriptors.Add(descriptor);
            }

            tool = new ToolDescriptor(name, description, parameterDescriptors);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Adds all the public static methods exposed from DLL as function tool
        /// </summary>
        /// <param name="dllpath">Full path of the DLL</param>
        /// <returns>List of the tools name that got added to the tools collection.</returns>
        public static List<string> AddToolsFromDLL(string dllpath) 
        {
            var toolset = new DLLFunctionTools(dllpath);

            toolsCollection.AddTools(toolset);

            return toolset.Select(t => t.Name).ToList();
        }

        /// <summary>
        /// When the language model returns a function_call message for any tool, that tool
        /// needs to be executed by client and the response of that execution can be registered
        /// using this method.
        /// </summary>
        /// <param name="sessionid">The session id for the conversation.</param>
        /// <param name="toolName">Name of the function that was executed.</param>
        /// <param name="response">Output returned by the given tool as string(json).</param>
        /// <param name="temperature">A value between 0 and 1 to control the randomness of the response.</param>
        /// <returns>The response from the language model.</returns>
        public static string AddToolResponseToConversation(string sessionid, string toolName, string response, double temperature)
        {
            var chat = GetAIService().GetConversation(sessionid);

            if(chat == null)
            {
                return $"ERROR: Invalid session id, there is no existing conversation with the session id {sessionid}";
            }

            chat.AppendMessage(new FunctionMessage(toolName, response));

            var msg = chat.GetResponseAsync(temperature).GetAwaiter().GetResult();
            if (msg is FunctionCallMessage)
            {
                return FunctionTool.ToJsonString(msg);
            }

            return msg.content;
        }

        /// <summary>
        /// Executes a given tool with the arguments passed and returns the 
        /// output as a string. For complex objects it returns JSON string.
        /// </summary>
        /// <param name="name">Name of the tool to execute</param>
        /// <param name="executionContext">A JSON string with key value pairs as a context holding variable values to execute the tool.</param>
        /// <returns>Output string as a result of the execution if successful, else error message starting with ERROR.</returns>
        public static string ExecuteTool(string name, string executionContext)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool == null) return $"ERROR: A tool with name, '{name}' doesn't exists!!";

            var ctx = FunctionTool.Deserialize<Dictionary<string, object>>(executionContext);
            if (ctx == null) return $"ERROR: Invalid execution context!!";

            exeResults = new ExecutionContext(ctx);

            return tool.ExecuteAsync(exeResults).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a given tool with the arguments passed and returns the 
        /// output as a string. For complex objects it returns JSON string.
        /// </summary>
        /// <param name="sessionName">Name of the session to access memory store</param>
        /// <param name="toolName">Name of the tool to execute</param>
        /// <param name="executionContext">A JSON string with key value pairs as a context holding variable values to execute the tool.</param>
        /// <returns>Output string as a result of the execution if successful, else error message starting with ERROR.</returns>
        public static string ExecuteTool(string sessionName, string toolName, string executionContext)
        {
            var tool = toolsCollection.GetTool(toolName);
            if (tool == null) return $"ERROR: A tool with name, '{toolName}' doesn't exists!!";

            var ctx = FunctionTool.Deserialize<Dictionary<string, object>>(executionContext);
            if (ctx == null) return $"ERROR: Invalid execution context!!";

            IMemoryStore memory = null;
            if(!memories.TryGetValue(sessionName, out memory))
            {
                memory = new MemoryStore();
                memories.Add(sessionName, memory);
            }
            
            //Create execution context with memory
            exeResults = new ExecutionContext(ctx).WithMemory(memory);

            return tool.ExecuteAsync(exeResults).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Restores a session with the given session file.
        /// </summary>
        /// <param name="sessionName">Name of the session</param>
        /// <param name="sessionFilePath">Full path of the session file</param>
        /// <returns>Status of the operation</returns>
        public static string RestoreSession(string sessionName, string sessionFilePath) 
        {
            var memory = MemoryStore.FromJsonFile(sessionFilePath);
            if (null == memory) return $"ERROR: Failed to restore session: {sessionName} with file: {sessionFilePath}";

            memories[sessionName] = memory;
            return "success";
        }

        /// <summary>
        /// Saves a given session to a file.
        /// </summary>
        /// <param name="sessionName">Name of the session</param>
        /// <param name="sessionFilePath">Full path of the session file to save the session.</param>
        /// <returns>Status of the operation</returns>
        public static string SaveSession(string sessionName, string sessionFilePath)
        {
            IMemoryStore memory = null;
            if(!memories.TryGetValue(sessionName, out memory))
            {
                return $"ERROR: Session not found: {sessionName}";
            }

            memory.Save(sessionFilePath);
            return "success";
        }

        /// <summary>
        /// Gets the execution result for a given tool. Useful in getting intermediate results.
        /// </summary>
        /// <param name="toolname">Name of the tool</param>
        /// <returns>Execution result if available, else error statement.</returns>
        public static string GetExecutionResult(string toolname)
        {
            object retval = null;
            if(exeResults.TryGetResult(toolname, out retval))
            {
                return FunctionTool.ToJsonString(retval);
            }

            return $"ERROR: Couldn't find execution result for tool: {toolname}";
        }

        /// <summary>
        /// Returns a list of input parameters for the given tool.
        /// </summary>
        /// <param name="toolName">Name of the tool</param>
        /// <returns>List of input parameters</returns>
        public static List<string> GetToolInputParameters(string toolName)
        {
            var variables = new List<string>();
            var tool = toolsCollection.GetTool(toolName);
            if (tool == null) return variables;

            return tool.Descriptor.InputParameters.ToList();
        }

        /// <summary>
        /// Returns the type of a given input parameter.
        /// </summary>
        /// <param name="toolName">Name of the tool</param>
        /// <param name="parameter">Name of the parameter</param>
        /// <returns>Type of parameter such as string, number, integer, boolean, array etc.</returns>
        public static string GetToolInputParameterType(string toolName, string parameter) 
        {
            var tool = toolsCollection.GetTool(toolName);
            if (tool == null) return $"ERROR: A tool with name, '{toolName}' doesn't exists!!";

            var p = tool.Descriptor.Parameters.Properties.Find(x => string.Equals(x.Name, parameter, StringComparison.OrdinalIgnoreCase));

            if (p == null) return $"ERROR: A parameter with name '{parameter}' doesn't exists with tool '{toolName}'";

            return p.Type.Type;
        }

        /// <summary>
        /// Removes the given tool from the toolset.
        /// </summary>
        /// <param name="toolName">Name of the tool</param>
        /// <returns>Status of the operation</returns>
        public static string RemoveTool(string toolName)
        {
            var tool = toolsCollection.GetTool(toolName);
            if (tool == null) return $"ERROR: A tool with name, '{toolName}' doesn't exists!!";

            if (toolsCollection.RemoveTool(toolName)) return "success";

            return $"ERROR: Failed to remove the tool {toolName}";
        }

        #region Tools Creation

        /// <summary>
        /// Creates a prompt tool and adds it to tools collection. The input parameters to
        /// execute this tool is same as the variables defined in the input template.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="template">Template string with input variables. The variable input
        /// must be represented by {{$input}} text in the template, where
        /// input is the variable value in the template.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreatePromptTool(string name, string description, string template) 
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = PromptTool.WithTemplate(template).WithName(name).WithDescription(description);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a query tool and adds it to tools collection. The query tool makes use of
        /// registered language model to get response of the query. The input parameters to execute
        /// this tool is same as the variables defined in the input template.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="template">Template string with input variables. The variable input
        /// must be represented by {{$input}} text in the template, where
        /// input is the variable value in the template.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateQueryTool(string name, string description, string template)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = QueryTool.WithPromptTemplate(template)
                            .WithLanguageModel(LanguageModel)
                            .WithName(name)
                            .WithDescription(description);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a pipeline with a list of tools. A pipeline executes tools sequentially, where
        /// output of previous tools can be input of the next tool in the sequence. The input
        /// parameter to execute this tool is same as the input parameter of the first tool in
        /// the pipeline.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="tools">List of tools name to be added to the pipeline.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateToolsPipeline(string name, string description, List<string> tools)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            var toolChain = new List<IFunctionTool>();

            foreach (var item in tools)
            {
                tool = toolsCollection.GetTool(item);
                if (tool == null) return $"ERROR: A tool with name, '{item}' doesn't exists!!";

                toolChain.Add(tool);
            }

            tool = Pipeline.WithTools(toolChain)
                           .WithName(name)
                           .WithDescription(description);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates MapReduce tool which can take a mapper tool to map the input collection to
        /// an intermediate data using mapper tool and then reduce the intermediate data to the final
        /// output using reducer tool. The mapper runs in parallel, hence mapper tool needs to ensure
        /// thread safety. The input parameter to execute this tool is the same as the input parameters
        /// of the mapper tool, however, each parameter is of array type in this case.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="mapper">Name of the tool to map the input collection to an intermediate data.</param>
        /// <param name="reducer">Name of the tool to process the intermediate data to reduce to the final output.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateMapReduceTool(string name, string description, string mapper, string reducer)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            var maptool = toolsCollection.GetTool(mapper);
            if (maptool == null) return $"ERROR: A tool with name, '{mapper}' doesn't exists!!";

            var reducetool = toolsCollection.GetTool(reducer);
            if (reducetool == null) return $"ERROR: A tool with name, '{reducer}' doesn't exists!!";

            tool = MapReduceTool.WithMapperReducer(maptool, reducetool)
                                .WithName(name)
                                .WithDescription(description);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a combine tool to combine an array of string using newline. The input parameter
        /// name to execute this tool is 'input'
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateCombineTool(string name, string description)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = CombineTool.Create().WithName(name).WithDescription(description);

            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a bing search tool to search specific query. The input parameter name to
        /// execute this tool is 'query'.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="apikey">API key for Bing Search</param>
        /// <param name="maxResultsCount">Count of maximum results to be expected by this search tool.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateBingSearchTool(string name, string description, string apikey, int maxResultsCount) 
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = SearchTool.ForBingSearch(apikey)
                             .WithMaxResultCount(maxResultsCount)
                             .WithName(name)
                             .WithDescription(description);

            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a search tool for semantic search using a given vector database. The input parameter
        /// name to execute this tool is 'query'.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="dbpath">Full path of the vector database. The vector database can be created using 
        /// CreateVectorDatabaseForSemanticSearch method.</param>
        /// <param name="maxResultsCount">Count of maximum results to be expected by this search tool.</param>
        /// <returns>Status of the operation.</returns>
        public static string CreateSemanticSearchTool(string name, string description, string dbpath, int maxResultsCount)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = SearchTool.ForSemanticSearchFromDatabase(dbpath)
                             .WithMaxResultCount(maxResultsCount)
                             .WithName(name)
                             .WithDescription(description);

            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates HttpTool that can be used to make GET/POST/PUT/DELETE requests to any uri. The input
        /// parameters to execute this tool are 'uri', 'method', and 'body'. 'method' should be one of the
        /// method type GET, POST, PUT, or DELETE.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="requestHeaderJson">The key value pairs for header information in a JSON format.</param>
        /// <returns>Status of the operation</returns>
        public static string CreateHttpRequestTool(string name, string description, string requestHeaderJson)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            var httpTool = HttpTool.WithClient(null);

            if (!string.IsNullOrEmpty(requestHeaderJson))
            {
                var headers = FunctionTool.Deserialize<Dictionary<string, string>>(requestHeaderJson);

                if (headers == null) return $"ERROR: Invalid requestHeaderJson, unable to parse the json as dictionary!!";

                httpTool=  httpTool.WithDefaultRequestHeaders(headers);
            }
            
            tool = httpTool.WithName(name).WithDescription(description);

            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a text extractor tool that extracts text from different files. Currently
        /// txt, csv and pdf files are supported. The input parameter to execute this tool
        /// is 'input'.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <returns>Status of the operation</returns>
        public static string CreateTextExtractorTool(string name, string description)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = TextExtractorTool.Create().WithName(name).WithDescription(description);
            toolsCollection.AddTool(tool);

            return "success";
        }

        /// <summary>
        /// Creates a tool that can extract data from an unstructured text using language model. The input
        /// parameter name to execute this tool is 'input'.
        /// </summary>
        /// <param name="name">Name of the tool, it must be unique.</param>
        /// <param name="description">Description of the tool.</param>
        /// <param name="parametersJson">The key value pairs for name and description for parameters to be extracted
        /// in a JSON format.</param>
        /// <returns>Status of the operation</returns>
        public static string CreateDataExtractorTool(string name, string description, string parametersJson)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            if (!string.IsNullOrEmpty(parametersJson))
            {
                var parameters = FunctionTool.Deserialize<Dictionary<string, string>>(parametersJson);

                if (parameters == null) return $"ERROR: Invalid parametersJson, unable to parse the json as dictionary!!";

                tool = DataExtractorTool.Create().WithParameters(parameters).WithName(name).WithDescription(description);
                
                toolsCollection.AddTool(tool);
                return "success";
            }

            return $"ERROR: Invalid parametersJson, unable to parse the json as dictionary!!";
        }

        /// <summary>
        /// Creates a text summarizer tool with map reduce strategy. The input parameter to
        /// execute this tool is input.
        /// </summary>
        /// <param name="name">Name of the tool</param>
        /// <param name="description">Description of the tool</param>
        /// <param name="mapperPrompt">Prompt to summarize each chunk from a large text.</param>
        /// <param name="reducerPrompt">Prompt to combine all the summaries to create final summary.</param>
        /// <returns>Status of the operation</returns>
        public static string CreateSummarizerTool(string name, string description, string mapperPrompt, string reducerPrompt)
        {
            var tool = toolsCollection.GetTool(name);
            if (tool != null) return $"ERROR: A tool with name, '{name}' already exists!!";

            tool = TextSummarizer.WithMapReduce(mapperPrompt, reducerPrompt)
                .WithLanguageModel(LanguageModel)
                .WithName(name)
                .WithDescription(description);

            toolsCollection.AddTool(tool);
            return "success";
        }

        /// <summary>
        /// Creates an automation agent which can perform a given objective with the help of tools available with it.
        /// </summary>
        /// <param name="name">Name of the agent</param>
        /// <param name="allowedTools">Lists of tools that agent can use to perform an objective.</param>
        /// <param name="maxAllowedSteps">Maximum number of steps the agent is allowed to execute.</param>
        /// <param name="workingdirectory">Working directory for agent to store temporary data.</param>
        /// <returns>Status of the operation</returns>
        public static string CreateAgent(string name, List<string> allowedTools, int maxAllowedSteps, string workingdirectory)
        {
            var tools = allowedTools.Select(x => toolsCollection.GetTool(x)).Where(t => t != null).ToList();

            var agent = Agent.Create(name, workingdirectory)
                .WithLanguageModel(LanguageModel)
                .WithTools(tools)
                .WithMaxAllowedSteps(maxAllowedSteps);

            agents.Add(name, agent);

            return "success";
        }

        /// <summary>
        /// Executes a given objective.
        /// </summary>
        /// <param name="agent">Name of the agent</param>
        /// <param name="objective">Objective to achieve</param>
        /// <param name="temperature">A value between 0 and 1 to define creativity.</param>
        /// <returns>Agent's response for the given objective as a JSON object of the following format. 
        /// { "tool": "Tool's Name", "parameters": {"INPUT": "some input", "PARAMETER_NAME": "some value"}}
        /// If excution is complete then the tool will be "FinishAction" else
        /// the agent will return an action that client can execute and update agent with response
        /// using UpdateAgentActionResponse method.</returns>
        public static string PlanAndExecuteWithAgent(string agent, string objective, double temperature)
        {
            Agent worker = null;

            if (!agents.TryGetValue(agent, out worker))
            {
                var error = new StepAction() { 
                    tool = "FinishAction", 
                    parameters = new Dictionary<string, object>() { 
                        { "error", $"ERROR: Agent: '{agent}' not found!!" } 
                    } 
                };
                return FunctionTool.ToJsonString(error);
            }

            worker.WithTemperature(temperature); //update the temperature parameter
            var step = worker.ExecuteAsync(objective).GetAwaiter().GetResult();

            var result = new StepAction()
            {
                tool = step.Tool.Name,
                parameters = new Dictionary<string, object>(step.ExecutionContext.GetParameters())
            };

            return FunctionTool.ToJsonString(result);
        }

        /// <summary>
        /// Updates the agent action response with the agent and executes the rest of the plan.
        /// </summary>
        /// <param name="agent">Name of the agent</param>
        /// <param name="toolName">Name of the tool that has executed</param>
        /// <param name="output">output of the tool</param>
        /// <returns>Agent's response for the given objective as a JSON object of the following format. 
        /// { "tool": "Tool's Name", "parameters": {"INPUT": "some input", "PARAMETER_NAME": "some value"}}
        /// If excution is complete then the tool will be "FinishAction" else
        /// the agent will return an action that client can execute and update agent with response
        /// using UpdateAgentActionResponse method.</returns>
        public static string UpdateAgentActionResponse(string agent, string toolName, string output)
        {
            Agent worker = null;

            if (!agents.TryGetValue(agent, out worker))
            {
                var error = new StepAction()
                {
                    tool = "FinishAction",
                    parameters = new Dictionary<string, object>() {
                        { "error", $"ERROR: Agent: '{agent}' not found!!" }
                    }
                };
                return FunctionTool.ToJsonString(error);
            }

            var step = worker.UpdateAgentActionResponseAsync(toolName, output).GetAwaiter().GetResult();

            var result = new StepAction()
            {
                tool = step.Tool.Name,
                parameters = new Dictionary<string, object>(step.ExecutionContext.GetParameters())
            };

            return FunctionTool.ToJsonString(result);
        }

        #endregion
    }
}
