using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Agents
{
    /// <summary>
    /// Represents agent action that can be executed as current step. It holdes a tool
    /// and execution context so that the tool can be executed.
    /// </summary>
    public class AgentAction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Execution Context</param>
        protected AgentAction(ExecutionContext context)
        {
            ExecutionContext = context;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tool">The tool this action can execute.</param>
        /// <param name="executionContext">Execution context to execute the tool.</param>
        /// <param name="thought">The reasoning of the thought for this action.</param>
        public AgentAction(IFunctionTool tool, ExecutionContext executionContext, string thought)
        {
            Tool = tool;
            ExecutionContext = executionContext;
            Thought = thought;
        }

        /// <summary>
        /// List of input parameters required for the tool/action.
        /// </summary>
        public virtual IEnumerable<string> Parameters => Tool.Descriptor.InputParameters;

        /// <summary>
        /// Gets the tool that this action can execute.
        /// </summary>
        public IFunctionTool Tool { get; protected set; }

        /// <summary>
        /// Gets the reasoning for this action.
        /// </summary>
        public string Thought { get; set; }

        /// <summary>
        /// Execution context for this action
        /// </summary>
        public ExecutionContext ExecutionContext { get; private set; }

        /// <summary>
        /// Executes the given tool asynchronously.
        /// </summary>
        /// <returns>Output string returned from the tool after execution.</returns>
        public virtual async Task<string> ExecuteAsync()
        {
            return await Tool.ExecuteAsync(ExecutionContext);
        }
    }

    /// <summary>
    /// Represents the final action that holds the final output or error string if there was an error.
    /// </summary>
    public class FinishAction : AgentAction
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The final output</param>
        /// <param name="error">The error string</param>
        public FinishAction(string output, string error = "") : base(new ExecutionContext())
        {
            ExecutionContext["output"] = output;
            ExecutionContext["error"] = error;
            Thought = output;
            var parameters = new[]
            {
                new ParameterDescriptor(){Name = "output", Description = "output string"},
                new ParameterDescriptor(){Name = "error", Description = "Error string"}
            };

            Tool = new ToolDescriptor("FinishAction", "The final action", parameters);
        }

        /// <summary>
        /// Gets the output
        /// </summary>
        public string Output
        {
            get
            {
                object output;
                ExecutionContext.TryGetValue("output", out output);
                return (string)output;
            }
        }

        /// <summary>
        /// Gets the error string if the agent has met with an error.
        /// </summary>
        public string Error
        {
            get
            {
                object value;
                ExecutionContext.TryGetValue("error", out value);
                return (string)value;
            }
        }

        /// <summary>
        /// List of input parameters
        /// </summary>
        public override IEnumerable<string> Parameters => new[] { "output", "error" };

        /// <summary>
        /// Executes the given action
        /// </summary>
        /// <returns>Output string</returns>
        public override async Task<string> ExecuteAsync()
        {
            var output = ExecutionContext["output"];
            return await Task.FromResult(FunctionTool.ToJsonString(output));
        }
    }
}
