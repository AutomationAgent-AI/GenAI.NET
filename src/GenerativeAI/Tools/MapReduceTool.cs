using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Implements a map reduce tool, which can take a mapper tool to map the input collection to
    /// an intermediate data using mapper tool and then reduce the intermediate data to the final
    /// output using reducer tool. The mapper runs in parallel, hence mapper tool needs to ensure
    /// thread safety.
    /// </summary>
    public class MapReduceTool : FunctionTool
    {
        private IFunctionTool mapper;
        private IFunctionTool reducer;

        /// <summary>
        /// Constructor
        /// </summary>
        private MapReduceTool(IFunctionTool mapper, IFunctionTool reducer)
        {
            this.mapper = mapper;
            this.reducer = reducer;
            Name = "MapReduce";
            Description = $"Maps the input data using tool: {mapper.Name} and then reduces the intermediate data using tool: {reducer.Name}";
        }

        /// <summary>
        /// Creates an instance of MapReduceTool using mapper and reducer tool.
        /// </summary>
        /// <param name="mapper">A tool to map the input collection to an intermediate data.</param>
        /// <param name="reducer">A tool to process the intermediate data to reduce to the final output.</param>
        /// <returns>MapReduceTool</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static MapReduceTool WithMapperReducer(IFunctionTool mapper, IFunctionTool reducer)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            if (reducer == null) throw new ArgumentNullException("reducer");

            return new MapReduceTool(mapper, reducer);
        }

        /// <summary>
        /// Expands the given context to a collection of contexts, each context containing a
        /// single value of each of the parameter.
        /// </summary>
        /// <param name="context">Original context which has a collection of values for each of the parameters.</param>
        /// <param name="parameters">List of input parameters.</param>
        /// <returns>List of ExecutionContext</returns>
        /// <exception cref="ArgumentException"></exception>
        private IEnumerable<ExecutionContext> Expand(ExecutionContext context, IEnumerable<string> parameters)
        {
            List<ExecutionContext> result = null;
            bool createctx = true;
            
            foreach (var parameter in parameters)
            {
                object value = null;
                if (!context.TryGetValue(parameter, out value)) continue;

                if (!(value is IEnumerable)) throw new ArgumentException($"Expected an Array of objects for parameter: {parameter}!");

                var values = (value as IEnumerable).Cast<object>().ToList();
                if (createctx)
                {
                    result = values.Select(v => new ExecutionContext(new Dictionary<string, object>() { { parameter, v } })).ToList();
                    createctx = false;
                    continue;
                }
                else if (values.Count != result.Count) throw new ArgumentException($"All parameter values are not of the same size. Size of parameter: {parameter} is {values.Count}, whereas expected size was {result.Count}!!");

                int i = 0;
                foreach (var v in values)
                {
                    result[i++][parameter] = v;
                }
            }

            return result;
        }


        /// <summary>
        /// Implements the core logic to execute the map reduce operations asynchronously
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            // 1. Expand the input context for parallel execution.
            var ctxts = Expand(context, mapper.Descriptor.InputParameters);

            // 2. Concurrent storage for intermediate data.
            ConcurrentBag<string> mappedResults = new ConcurrentBag<string>();

            // 3. Parallel execution of mapper tool over the expanded context.
            Parallel.ForEach(ctxts, ctxt =>
            {
                var r = mapper.ExecuteAsync(ctxt).GetAwaiter().GetResult();
                mappedResults.Add(r);
            });

            // 4. Create new context with the mapped results for reduce operation
            var reducerContext = new ExecutionContext();
            reducerContext[reducer.Descriptor.InputParameters.First()] = ctxts.Select(c => c[$"Result.{mapper.Name}"]);

            // 5. Execute the reduce operation asynchronously
            var result = new Result() { success = true };
            var output = await reducer.ExecuteAsync(reducerContext);

            // 6. Get the original result object from the reducerContext if present, else use the returned output above.
            object obj = null;
            if(reducerContext.TryGetResult(reducer.Name, out obj))
            {
                result.output = obj;
            }
            else
            {
                result.output = output;
                if (output.StartsWith("ERROR:"))
                {
                    result.success = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Overrides the GetDescriptor method to return it's FunctionDescriptor. This
        /// tool has input parameters same as mapper tool but of type array.
        /// </summary>
        /// <returns>FunctionDescriptor</returns>
        protected override FunctionDescriptor GetDescriptor()
        {
            var mapperParameters = mapper.Descriptor.Parameters.Properties;

            var parameters = mapperParameters.Select(
                p => new ParameterDescriptor()
                {
                    Name = p.Name,
                    Description = p.Description,
                    Type = new ArrayTypeDescriptor(p.Type),
                    Required = p.Required,
                });

            var function = new FunctionDescriptor(Name, Description, parameters);

            return function;
        }
    }
}
