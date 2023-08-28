using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Represents a simple collection of tools as a toolset
    /// </summary>
    public class ToolsCollection : IFunctionToolSet
    {
        private readonly Dictionary<string, IFunctionTool> toolsDictionary;

        private string GetName(IFunctionTool tool)
        {
            var name = tool.Name;
            if (string.IsNullOrEmpty(tool.Name))
            {
                name = tool.GetType().Name;
            }

            return name;
        }

        internal ToolsCollection() { toolsDictionary = new Dictionary<string, IFunctionTool>(); }

        /// <summary>
        /// Constructs the ToolsCollection with a single tool.
        /// </summary>
        /// <param name="tool">IFunctionTool to be added to the toolset/collection.</param>
        public ToolsCollection(IFunctionTool tool)
        {
            toolsDictionary = new Dictionary<string, IFunctionTool>() { { GetName(tool), tool } };
        }

        /// <summary>
        /// Constructs the ToolsCollection with a list of tools.
        /// </summary>
        /// <param name="tools">A list of IFunctionTool to be added to the collection.</param>
        public ToolsCollection(IEnumerable<IFunctionTool> tools) 
        {
            toolsDictionary = tools.ToDictionary(t => GetName(t));
        }

        /// <summary>
        /// Adds a given tool to the collection.
        /// </summary>
        /// <param name="tool">IFunctionTool to be added to the toolset/collection.</param>
        public void AddTool(IFunctionTool tool)
        {
            toolsDictionary.Add(GetName(tool), tool);
        }

        /// <summary>
        /// Adds a given list of tools to the collection.
        /// </summary>
        /// <param name="tools">A list of IFunctionTool to be added to the collection.</param>
        public void AddTools(IEnumerable<IFunctionTool> tools)
        {
            foreach (var item in tools)
            {
                toolsDictionary.Add(GetName(item), item);
            }
        }

        /// <summary>
        /// Removes a tool with given name from the collection.
        /// </summary>
        /// <param name="name">Name of the tool to be removed from the collection.</param>
        /// <returns>True if a tool is successfully found in the collection and removed.</returns>
        public bool RemoveTool(string name)
        {
            return toolsDictionary.Remove(name);
        }

        /// <summary>
        /// Get a tool with given name, throws exception if the given name is not present in the collection.
        /// </summary>
        /// <param name="name">Name of the tool.</param>
        /// <returns>IFunctionTool if present</returns>
        public IFunctionTool this[string name] => toolsDictionary[name];

        /// <summary>
        /// Executes a given function with the arguments passed and returns the 
        /// output as a string. For complex objects it returns JSON string.
        /// </summary>
        /// <param name="functionName">Name of the function to execute</param>
        /// <param name="context">Execution context holding arguments of the function</param>
        /// <returns>Output string as a result of the execution</returns>
        public async Task<string> ExecuteAsync(string functionName, ExecutionContext context)
        {
            IFunctionTool tool = null;

            if (!toolsDictionary.TryGetValue(functionName, out tool)) throw new ArgumentException($"{functionName}, not found in the toolset!");

            return await tool.ExecuteAsync(context);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IFunctionTool> GetEnumerator()
        {
            return toolsDictionary.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns function descriptors for all the public static functions of a class
        /// this toolset wraps.
        /// </summary>
        /// <returns>A list of function descriptors</returns>
        public IEnumerable<FunctionDescriptor> GetFunctions()
        {
            return toolsDictionary.Select(p => p.Value.Descriptor);
        }

        /// <summary>
        /// Gets a tool for a specific name.
        /// </summary>
        /// <param name="name">Name of the tool to search</param>
        /// <returns>IFunctionTool instance if present in the collection.</returns>
        public IFunctionTool GetTool(string name)
        {
            IFunctionTool tool = null;

            toolsDictionary.TryGetValue(name, out tool);

            return tool;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return toolsDictionary.Values.GetEnumerator();
        }
    }
}
