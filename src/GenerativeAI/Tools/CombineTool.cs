using Automation.GenerativeAI.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    public class CombineTool : FunctionTool
    {
        private ParameterDescriptor input = new ParameterDescriptor() { 
            Name = "input", 
            Description = "List of strings to combine.",
            Type = new ArrayTypeDescriptor(TypeDescriptor.StringType),
            Required = true,
        };

        public CombineTool() 
        { 
            Name = "Combine";
            Description = "Combines a list of string into a single string using newline";
        }

        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            object value;
            if(context.TryGetValue(input.Name, out value))
            {
                IEnumerable<string> values = value as IEnumerable<string>;
                var result = new Result() { success = true };
                result.output = string.Join("\n\n", values);

                return await Task.FromResult(result);
            }

            return await Task.FromResult(new Result() {  success = false });
        }

        protected override FunctionDescriptor GetDescriptor()
        {
            return new FunctionDescriptor(Name, Description, new[] {input});
        }
    }
}
