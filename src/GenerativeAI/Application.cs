using Automation.GenerativeAI.Chat;
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
using ExecutionContext = Automation.GenerativeAI.Interfaces.ExecutionContext;

namespace Automation.GenerativeAI
{
    class DummyTool : IFunctionTool
    {
        public DummyTool(FunctionDescriptor descriptor) 
        {
            Descriptor = descriptor;
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
        /// Gets AI assitant's response for a given message. This request doesn't have any memory.
        /// </summary>
        /// <param name="message">User's message</param>
        /// <param name="temperature">A value between 0 to 2, that controls randomness of the response. 
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
        /// <param name="temperature"></param>
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
        /// <param name="temperature">A value between 0 and 2 to control the randomness of the response.</param>
        /// <returns>Returns the AI assitant's response.</returns>
        /// <exception cref="System.Exception"></exception>
        public static string GetResponseFromContext(string sessionid, string systemctx, string vdbpath, string message, double temperature)
        {
            return GetResponseFromContextAsync(sessionid, systemctx, vdbpath, message, temperature).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Creates a function description that can be passed to language model
        /// </summary>
        /// <param name="name">Name of the function</param>
        /// <param name="description">Description of the function</param>
        /// <param name="parametersTableCSVAsText">A table to contain parameter name, description and type</param>
        /// <returns>Returns function id</returns>
        public static int CreateFunctionDescription(string name, string description, string parametersTableCSVAsText)
        {
            List<ParameterDescriptor> parameterDescriptors = new List<ParameterDescriptor>();

            var lines = parametersTableCSVAsText.Split('\n');

            var indices = new Dictionary<string, int>() { { "name", 0 }, { "description", 1 }, { "type", 2 } };
            int rowindex = 0;
            foreach(var line in lines)
            {
                var props = line.Split(',');
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
                        case "integer":
                            descriptor.Type = TypeDescriptor.IntegerType; break;
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

            var function = new FunctionDescriptor(name, description, parameterDescriptors);
            var tool = new DummyTool(function);
            toolsCollection.AddTool(tool);

            return toolsCollection.Count();
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
        /// Adds a function message as a response to function_call message.
        /// </summary>
        /// <param name="sessionid">The session id for the conversation.</param>
        /// <param name="functionName">Name of the function that was executed.</param>
        /// <param name="message">Output returned from the function call as string(json).</param>
        /// <param name="temperature">A value between 0 and 2 to control the randomness of the response.</param>
        /// <returns>The response from the language model.</returns>
        public static string AddFunctionMessage(string sessionid, string functionName, string message, double temperature)
        {
            var chat = GetAIService().GetConversation(sessionid);

            if(chat == null)
            {
                return $"ERROR: Invalid session id, there is no existing conversation with the session id {sessionid}";
            }

            chat.AppendMessage(new FunctionMessage(functionName, message));

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
        /// Creates a prompt tool and adds it to tools collection.
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
        /// registered language model to get response of the query.
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
        /// output of previous tools can be input of the next tool in the sequence.
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
        /// thread safety.
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
        /// Creates a combine tool to combine an array of string using newline.
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
        /// Creates a bing search tool to search specific query.
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
        /// Creates a search tool for semantic search using a given vector database
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
        /// Creates HttpTool that can be used to make GET/POST/PUT/DELETE requests to any uri.
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
        /// txt, csv and pdf files are supported.
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
        /// Creates a tool that can extract data from an unstructured text using language model.
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

        #endregion
    }
}
