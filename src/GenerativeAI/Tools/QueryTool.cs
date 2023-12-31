﻿using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// A Query tool that allows user to use a prompt template to query LLM
    /// </summary>
    public class QueryTool : PromptTool
    {
        ILanguageModel languageModel;
        double temperature = 0.8;

        /// <summary>
        /// Default constructor of QueryTool
        /// </summary>
        protected QueryTool()
        {
            Name = "Query";
            Description = "Performs query on a given prompt to the language model";
        }

        /// <summary>
        /// Creates QueryTool object with a prompt template object.
        /// </summary>
        /// <param name="prompt">Prompt template for the tool</param>
        /// <returns>QueryTool</returns>
        public static QueryTool WithPromptTemplate(PromptTemplate prompt) 
        {
            return new QueryTool()
            {
                prompt = prompt
            };
        }

        /// <summary>
        /// Creates QueryTool object with a prompt template string.
        /// </summary>
        /// <param name="template">Prompt template for the tool</param>
        /// <returns>QueryTool</returns>
        public static QueryTool WithPromptTemplate(string template)
        {
            return new QueryTool()
            {
                prompt = new PromptTemplate(template)
            };
        }

        /// <summary>
        /// Sets language model to the tool
        /// </summary>
        /// <param name="model">The language model implementation</param>
        /// <returns>QueryTool</returns>
        public QueryTool WithLanguageModel(ILanguageModel model)
        {
            this.languageModel = model;
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter for the tool to define the creativity.
        /// </summary>
        /// <param name="temperature">A value between 0 and 1 to define creativity</param>
        /// <returns>This QueryTool</returns>
        public QueryTool WithTemperature(double temperature)
        {
            this.temperature = temperature;
            return this;
        }

        private ILanguageModel LanguageModel
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

        /// <summary>
        /// Overrides the core executoion logic to execute this query tool with the given context
        /// </summary>
        /// <param name="context">Execution context wtih prompt parameters</param>
        /// <returns>Result</returns>
        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var result = new Result() { success = false, output = string.Empty };
            
            var msg = prompt.FormatMessage(context);
            if (msg != null)
            {
                context.MemoryStore.AddMessage(msg);
                var history = context.MemoryStore.ChatHistory(msg.content).ToList();
                
                var response = await LanguageModel.GetResponseAsync(history, temperature);
                
                result.output = response.Response;
                result.success = response.Type == ResponseType.Done;
                if(response.Type != ResponseType.Failed)
                {
                    msg = new ChatMessage(Role.assistant, response.Response);
                    context.MemoryStore.AddMessage(msg); 
                }
            }

            return result;
        }
    }
}
