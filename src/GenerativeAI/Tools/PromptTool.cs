using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// A simple tool to evaluate prommpt.
    /// </summary>
    public class PromptTool : FunctionTool
    {
        /// <summary>
        /// PromptTemplate for this tool
        /// </summary>
        protected PromptTemplate prompt;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected PromptTool() { }

        /// <summary>
        /// Creates a new instance of PrompTool with template string
        /// </summary>
        /// <param name="prompttemplate">Prompt template string.</param>
        /// <returns>A new PromptTool</returns>
        public static PromptTool WithTemplate(string prompttemplate)
        {
            return new PromptTool() { prompt = new PromptTemplate(prompttemplate) };
        }

        /// <summary>
        /// Executes this tool with the given context
        /// </summary>
        /// <param name="context">Execution context containing template variable values.</param>
        /// <param name="output">Output string</param>
        /// <returns>True if successful</returns>
        protected bool Execute(ExecutionContext context, out string output)
        {
            output = string.Empty;
            var msg = prompt.FormatMessage(context);
            if(msg == null) { return false; }

            output = msg.content;

            return true;
        }

        /// <summary>
        /// Overrides the core execution logic
        /// </summary>
        /// <param name="context">ExecutionContext</param>
        /// <returns>Result</returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            string output = string.Empty;
            var result = new Result();
            result.success = await Task.Run(() => Execute(context, out output));
            result.output = output;
            return result;
        }

        /// <summary>
        /// Returns a descriptor object for this tool
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            var parameters = prompt.Variables.Select(v => new ParameterDescriptor()
            {
                Name = v,
                Type = TypeDescriptor.StringType,
                Description = v
            });

            var function = new FunctionDescriptor(Name, Description, parameters.ToList());
            
            return function;
        }
    }
}
