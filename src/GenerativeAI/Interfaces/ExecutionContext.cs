using System.Collections.Generic;

namespace Automation.GenerativeAI.Interfaces
{
    /// <summary>
    /// Represents the execution context that holds all the parameters required for the execution.
    /// </summary>
    public class ExecutionContext
    {
        private Dictionary<string, object> parameters = null;

        internal IDictionary<string, object> GetParameters() { return parameters; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExecutionContext() 
        { 
            parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates ExecutionContext with given parameters
        /// </summary>
        /// <param name="parameters">Parameters dictionary</param>
        public ExecutionContext(Dictionary<string, object> parameters)
        {
            this.parameters = parameters;
        }

        /// <summary>
        /// Gets or Sets parameter value for a given parameter name.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <returns>Parameter value</returns>
        public object this[string name]
        {
            get => this.parameters[name];
            set
            {
                this.parameters[name] = value;
            }
        }

        /// <summary>
        /// Tries to get the result for specific tool
        /// </summary>
        /// <param name="toolname">Name of the tool</param>
        /// <param name="result">Execution result if available</param>
        /// <returns>True if successful</returns>
        public bool TryGetResult(string toolname, out object result)
        {
            object value = null;
            result = null;
            if(parameters.TryGetValue($"Result.{toolname}", out value))
            {
                result = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the result of the given tool to the context
        /// </summary>
        /// <param name="toolname">Name of the tool.</param>
        /// <param name="result">Execution Result of the tool</param>
        public void AddResult(string toolname, object result)
        {
            parameters[$"Result.{toolname}"] = result;
        }

        internal bool TryGetValue(string variable, out object value)
        {
            return this.parameters.TryGetValue(variable, out value);
        }
    }
}
