using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// The base implementation of IFunctionTool interface as FunctionTool
    /// </summary>
    public abstract class FunctionTool : IFunctionTool
    {
        /// <summary>
        /// Represent the Result object that has success status and the output string.
        /// </summary>
        protected class Result
        {
            /// <summary>
            /// Flag to indicate if the execution result was successful.
            /// </summary>
            public bool success = false;

            /// <summary>
            /// The output object as a result of execution of this FunctionTool
            /// </summary>
            public object output = null;
        }

        /// <summary>
        /// Descriptor for this tool
        /// </summary>
        protected FunctionDescriptor descriptor;

        protected FunctionTool()
        {
            Name = this.GetType().Name;
            Description = $"Executes a tool: {Name}";
        }

        /// <summary>
        /// Updates the name of the tool
        /// </summary>
        /// <param name="name">Name of the tool to update</param>
        /// <returns>The updated FunctionTool object</returns>
        public FunctionTool WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Updates description of the tool
        /// </summary>
        /// <param name="description">Description of the tool to be used for discovery</param>
        /// <returns>The updated FunctionTool object</returns>
        public FunctionTool WithDescription(string description)
        {
            if(!string.IsNullOrEmpty(description))
            {
                Description = description;
            }
            return this;
        }

        /// <summary>
        /// Name of the tool
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the tool
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the function descriptor used for tool discovery by agents and LLM
        /// </summary>
        public FunctionDescriptor Descriptor
        {
            get 
            { 
                if(descriptor == null) { descriptor = GetDescriptor(); }
                return descriptor;
            }
        }

        /// <summary>
        /// Returns function descriptor for the tool to make it discoverable by Agent
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected abstract FunctionDescriptor GetDescriptor();

        /// <summary>
        /// Executes the tool with given context
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>Result</returns>
        protected abstract Task<Result> ExecuteCoreAsync(ExecutionContext context);

        /// <summary>
        /// Executes this tool asynchronously.
        /// </summary>
        /// <param name="context">ExecutionContext</param>
        /// <returns>Output string</returns>
        public async Task<string> ExecuteAsync(ExecutionContext context)
        {
            string output = $"ERROR: Failed to execute Tool: {this.Name}";
            object retval = null;
            bool success = false;
            try
            {
                if(ValidateParameterTypes(context, Descriptor.Parameters.Properties))
                {
                    Logger.WriteLog(LogLevel.Info, LogOps.Command, $"Started Executing tool: {Name}.");
                    var result = await ExecuteCoreAsync(context);
                    retval = result.output;
                    success = result.success;
                }
                else
                {
                    Logger.WriteLog(LogLevel.Error, LogOps.Result, $"Parameter validation for tool: {Name} has failed!");
                }
            }
            catch (System.Exception ex)
            {
                success = false;
                Logger.WriteLog(LogLevel.Error, LogOps.Exception, ex.Message);
                Logger.WriteLog(LogLevel.StackTrace, LogOps.Exception, ex.StackTrace);
            }
            finally
            {
                if (success)
                {
                    context.AddResult(Name, retval);
                    output = ToJsonString(retval);
                    Logger.WriteLog(LogLevel.Info, LogOps.Result, output.Substring(0, output.Length > 500 ? 500 : output.Length));
                }
                else
                {
                    output = $"ERROR: Failed to execute Tool: {this.Name}";
                }
            }
            return output;
        }

        private static bool IsEnumerable(Type type)
        {
            return type.GetInterface(typeof(IEnumerable).FullName) != null;
        }

        private static bool ValidateParameterTypes(ExecutionContext context, IEnumerable<ParameterDescriptor> parameters)
        {
            foreach(ParameterDescriptor parameter in parameters)
            {
                object data = null;
                bool found = context.TryGetValue(parameter.Name, out data);
                if (!parameter.Required && !found)
                {
                    context[parameter.Name] = null;
                    continue;
                }

                if(found && parameter.Type.Type == TypeDescriptor.StringType.Type)
                {
                    context[parameter.Name] = ToJsonString(data);
                }
                else if (data != null && parameter.Type is ArrayTypeDescriptor && IsEnumerable(data.GetType()))
                {
                    IEnumerable collection = (IEnumerable)data;
                    var elementType = ((ArrayTypeDescriptor)parameter.Type).ItemType.Type;
                    switch (elementType)
                    {
                        case "string":
                            context[parameter.Name] = collection.Cast<string>().ToList();
                            break;
                        case "number":
                            context[parameter.Name] = collection.Cast<double>().ToList();
                            break;
                        case "boolean":
                            context[parameter.Name] = collection.Cast<bool>().ToList();
                            break;
                        case "integer":
                            context[parameter.Name] = collection.Cast<int>().ToList();
                            break;
                        default:
                            break;
                    }
                    
                }
                else if(data != null && data is string)
                {
                    var value = Convert((string)data, parameter.Type);
                    if(value == null) return false;

                    context[parameter.Name] = value;
                }
            }

            return true;
        }

        /// <summary>
        /// Convert the given string data to a give type, either by parsing or deserializing from JSON.
        /// </summary>
        /// <param name="data">Input data as string</param>
        /// <param name="type">Type description for data conversion.</param>
        /// <returns>Object of the desired type.</returns>
        public static object Convert(string data, TypeDescriptor type)
        {
            switch (type.Type)
            {
                case "number":
                    double number = 0.0;
                    if (!double.TryParse(data, out number)) return null;
                    return number;
                case "boolean":
                    bool val = false;
                    if (!bool.TryParse(data, out val)) return null;
                    return val;
                case "integer":
                    int integer = 0;
                    if (!int.TryParse(data, out integer)) return null;
                    return integer;
                case "array":
                case "object":
                    object obj = null;
                    var serializer = new JavaScriptSerializer();
                    obj = serializer.DeserializeObject(data);
                    return obj;
                case "string":
                    return data;
                default:
                    break;
            }

            return null;
        }

        /// <summary>
        /// Utility method to serialize a given object to JSON string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON string</returns>
        public static string ToJsonString(object obj)
        {
            if(obj == null) return string.Empty;
            if (obj is string || obj.GetType().IsValueType) return obj.ToString();

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(obj);
            return json;
        }

        /// <summary>
        /// Utility method to Deserialize the given json string to specific object type.
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="json">Input JSON string</param>
        /// <returns>Deserialized object</returns>
        public static T Deserialize<T>(string json)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Utility method to check if the input string is a JSON string.
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>True if JSON</returns>
        public static bool IsJsonString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;

            str = str.Trim();
            if ((str.StartsWith("{") && str.EndsWith("}")) ||
                (str.StartsWith("[") && str.EndsWith("]"))) return true;

            return false;
        }
    }
}
