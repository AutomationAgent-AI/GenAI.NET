using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    public class TextSummarizer : FunctionTool
    {
        private QueryTool mapper;
        private QueryTool reducer;
        private ILanguageModel languageModel;
        private double temperature = 0.8;

        /// <summary>
        /// The input text parameter description for the TextSummarizer tool.
        /// </summary>
        public static readonly ParameterDescriptor Parameter = new ParameterDescriptor() { Name = "text", Description = "Text to summarize." };

        /// <summary>
        /// Creates a text summarizer tool with map reduce algorithm
        /// </summary>
        /// <param name="mapperPrompt">A prompt to summarize each chunk from the text.</param>
        /// <param name="reducerPrompt">A prompt to get combined summary.</param>
        /// <returns>TextSummarizer</returns>
        public static TextSummarizer WithMapReduce(string mapperPrompt = "", string reducerPrompt = "")
        {
            var tool = new TextSummarizer();
            tool.CreateMapperReducerTools(mapperPrompt, reducerPrompt);
            return tool;
        }

        /// <summary>
        /// Sets language model to the agent
        /// </summary>
        /// <param name="languageModel">Language model for agent to perform certain tasks</param>
        /// <returns>This TextSummarizer</returns>
        public TextSummarizer WithLanguageModel(ILanguageModel languageModel)
        {
            this.languageModel = languageModel;
            mapper.WithLanguageModel(languageModel);
            reducer.WithLanguageModel(languageModel);
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter for the tool to define the creativity.
        /// </summary>
        /// <param name="temperature">A value between 0 and 1 to define creativity</param>
        /// <returns>This TextSummarizer</returns>
        public TextSummarizer WithTemperature(double temperature)
        {
            this.temperature = temperature;
            return this;
        }

        /// <summary>
        /// Provide the language model for agent to work on. If a model is not set 
        /// it will use a default language model
        /// </summary>
        protected ILanguageModel LanguageModel
        {
            get
            {
                if (null == languageModel)
                {
                    languageModel = Application.DefaultLanguageModel;
                }

                return languageModel;
            }
        }

        private void CreateMapperReducerTools(string mapperPrompt, string redcerPrompt)
        {
            if (string.IsNullOrEmpty(mapperPrompt))
            {
                mapperPrompt = EmbeddedResource.GetrResource("Automation.GenerativeAI.Prompts.TextSummaryPrompt.txt");
            }
            if (string.IsNullOrEmpty(redcerPrompt))
            {
                redcerPrompt = EmbeddedResource.GetrResource("Automation.GenerativeAI.Prompts.SummaryOfSummaryPrompt.txt");
            }

            mapper = QueryTool.WithPromptTemplate(mapperPrompt)
                                  .WithLanguageModel(LanguageModel)
                                  .WithTemperature(temperature);

            reducer = QueryTool.WithPromptTemplate(redcerPrompt)
                                   .WithLanguageModel(LanguageModel)
                                   .WithTemperature(temperature);

            if (mapper.Descriptor.InputParameters.Count() > 1) throw new InvalidOperationException("Mapper is expected to have only one parameter!");
            if (reducer.Descriptor.InputParameters.Count() > 1) throw new InvalidOperationException("Reducer is expected to have only one parameter!");
        }

        private FunctionTool CreateMapReducePipeline()
        {
            var reducepipeline = Pipeline.WithTools(new IFunctionTool[] { CombineTool.Create(), reducer });

            return MapReduceTool.WithMapperReducer(mapper, reducepipeline);
        }

        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var result = new Result();
            object text = context[Parameter.Name];

            var splitter = TextSplitter.WithParameters(10000, 400);
            var txts = splitter.Split(TextObject.Create("SummarizationText", text.ToString())).Select(t => t.Text).ToList();

            var ctx = new ExecutionContext();
            ctx[mapper.Descriptor.InputParameters.First()] = txts;

            var tool = MapReduceTool.WithMapperReducer(mapper, CombineTool.Create().WithSkipText("NOT RELEVANT"));

            var combined = await tool.ExecuteAsync(ctx);

            ctx[reducer.Descriptor.InputParameters.First()] = combined;
            result.output = await reducer.ExecuteAsync(ctx);
            result.success = true;

            return result;
        }

        protected override FunctionDescriptor GetDescriptor()
        {
            return new FunctionDescriptor("TextSummarizer", "Summarizes the given text", new[] { Parameter });
        }
    }
}
