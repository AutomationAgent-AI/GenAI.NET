using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Represents a simple pipeline of tools, where output of previous tool is input of next tool.
    /// </summary>
    public class Pipeline : FunctionTool
    {
        private List<IFunctionTool> tools = new List<IFunctionTool>();

        private Pipeline() { }

        /// <summary>
        /// A method to create Pipeline with a list of tools. If any of the tool in the list can't be added 
        /// successfully, then that tool will be skipped from the pipeline.
        /// </summary>
        /// <param name="tools">A list of tools that needs to be executed sequntially.</param>
        /// <returns>Pipeline</returns>
        public static Pipeline WithTools(IEnumerable<IFunctionTool> tools)
        {
            var pipeline = new Pipeline();
            foreach (var tool in tools)
            {
                if (!pipeline.TryAddTool(tool)) continue;
            }

            return pipeline;
        }

        /// <summary>
        /// Tries to add a new tool to this pipeline. If the added tool is a follow
        /// up tool then it must have only one parameter. The pipeline can't resolve
        /// more than one parameters for execution.
        /// </summary>
        /// <param name="tool">A function tool to add</param>
        /// <returns>True if the tool is added successfully.</returns>
        public bool TryAddTool(IFunctionTool tool)
        {
            //Valide the tool if already more than one
            if(tools.Count > 0)
            {
                //If the follow up tool has more than one parameter then it can't be added
                var requiredParameters = GetMandatoryParameters(tool);
                if (requiredParameters.Count > 1)
                {
                    Logger.WriteLog(LogLevel.Warning, LogOps.Result, $"Tool: {tool.Name} has more than one input parameter, hence can't be added to the pipeline");
                    return false;
                }
            }

            tools.Add(tool);
            return true;
        }

        /// <summary>
        /// Gets a list of tools successfully added to the pipeline.
        /// </summary>
        public IEnumerable<IFunctionTool> Tools { get { return tools; } }

        /// <summary>
        /// Executes the tool with given context
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>Execution Result</returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var retval = new Result() { success = true, output = string.Empty };
            string result = string.Empty;
            var currentctx = context;
            foreach (IFunctionTool tool in tools)
            {
                currentctx = GetExecutionContext(tool, context, result);

                result = await tool.ExecuteAsync(currentctx);

                object data = null;
                if(!currentctx.TryGetResult(tool.Name, out data)) 
                {
                    data = result;
                }
                context.AddResult(tool.Name, data);
            }

            retval.output = result;
            return retval;
        }

        /// <summary>
        /// Returns function descriptor for the tool to make it discoverable by Agent
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            var parameters = new List<ParameterDescriptor>();
            if(tools.Any()) { parameters = tools.First().Descriptor.Parameters.Properties; }

            if (string.IsNullOrEmpty(Name))
            {
                Name = "Pipeline";
            }

            if (string.IsNullOrEmpty(Description))
            {
                Description = $"A pipline of tools: {tools.Select(t => t.Name).Aggregate((x, y) => $"{x}, {y}")}.";
            }

            return new FunctionDescriptor(Name, Description, parameters);
        }


        /// <summary>
        /// Gets the list of mandatory parameters for a given function tool. The parameters that
        /// are bound with input or result of other tool are not considered as mandatory parameters.
        /// </summary>
        /// <param name="tool">The tool</param>
        /// <returns>List of parameters</returns>
        private List<ParameterDescriptor> GetMandatoryParameters(IFunctionTool tool)
        {
            var parameters = tool.Descriptor.Parameters.Properties;
            return parameters.Where(p => p.Required && !p.Name.StartsWith("Result.") && !p.Name.StartsWith("Input.")).ToList();
        }


        private ExecutionContext GetExecutionContext(IFunctionTool tool, ExecutionContext context, string previousResult)
        {
            if (string.IsNullOrEmpty(previousResult)) return context;

            var newContext = new ExecutionContext();
            var parameters = tool.Descriptor.Parameters.Properties;
            foreach (var parameter in parameters)
            {
                if(parameter.Name.StartsWith("Input."))
                {
                    var name = parameter.Name.Substring(6);
                    newContext[parameter.Name] = context[name];
                }
                else if(parameter.Name.StartsWith("Result.")) 
                {
                    newContext[parameter.Name] = context[parameter.Name];
                }
                else
                {
                    newContext[parameter.Name] = previousResult;
                }
            }

            return newContext;
        }
    }
}
