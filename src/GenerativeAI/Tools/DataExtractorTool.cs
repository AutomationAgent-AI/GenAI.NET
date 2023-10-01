﻿using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Tools
{
    /// <summary>
    /// Extracts data from a given text based on the schema passed as json string.
    /// </summary>
    public class DataExtractorTool : FunctionTool
    {
        private readonly int ChunkingThreshold = 3000;
        private readonly int ChunkSize = 2000;
        private readonly int ChunkOverlap = 200;

        /// <summary>
        /// Creates basic DataExtractorTool
        /// </summary>
        /// <returns>A new DataExtractorTool</returns>
        public static DataExtractorTool Create()
        {
            return new DataExtractorTool() { Name = "DataExtractor" };
        }

        private PromptTemplate extratorPrompt = new PromptTemplate("Extract arguments and values from the following text only based on function specification provided, do not include extra parameter. {{$text}}");
        private double temperature = 0.8;

        /// <summary>
        /// Allows user to modify the default extractor prompt. The default prompt is as follows:
        /// "Extract arguments and values from the following text only based on function specification 
        /// provided, do not include extra parameter. {{$text}}"
        /// </summary>
        /// <param name="promptTemplate">Prompt template string with one input variable.</param>
        /// <returns>Updated DataExtractorTool</returns>
        public DataExtractorTool WithPrompt(string promptTemplate)
        {
            if(string.IsNullOrEmpty(promptTemplate))
            {
                extratorPrompt = new PromptTemplate(promptTemplate);
            }
            return this;
        }

        /// <summary>
        /// Allows user to override the temperature setting for data extraction. The default value is 0.8
        /// </summary>
        /// <param name="temperature">A value between 0 and 1 to control the randomness of the response.</param>
        /// <returns></returns>
        public DataExtractorTool WithTemperature(double temperature)
        {
            if (temperature > 0.0 && temperature <= 1.0)
            {
                this.temperature = temperature;
            }
            return this;
        }

        /// <summary>
        /// Sets language model to the tool
        /// </summary>
        /// <param name="model">The language model implementation</param>
        /// <returns>Updted DataExtractorTool</returns>
        public DataExtractorTool WithLanguageModel(ILanguageModel model)
        {
            this.languageModel = model;
            return this;
        }

        private List<ParameterDescriptor> parameters = new List<ParameterDescriptor>();

        /// <summary>
        /// Updates the parameters to extract
        /// </summary>
        /// <param name="parameters">Dictionary of parameter name and Description.</param>
        /// <returns>Updated DataExtractorTool</returns>
        public DataExtractorTool WithParameters(Dictionary<string, string> parameters)
        {
            foreach (var item in parameters)
            {
                var param = new ParameterDescriptor() { 
                    Name = item.Key, 
                    Description = item.Value
                };

                this.parameters.Add(param);
            }

            return this;
        }

        /// <summary>
        /// Updates the parameters to extract using json file or json string.
        /// </summary>
        /// <param name="json">json string or json file containing a list of Name and Description of parameters.</param>
        /// <returns>Updated DataExtractorTool</returns>
        public DataExtractorTool WithJSON(string json)
        {
            var jsontxt = json;
            if (File.Exists(json))
            {
                jsontxt = File.ReadAllText(json);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            try
            {
                var parameterDescriptors = serializer.Deserialize<List<ParameterDescriptor>>(jsontxt);
                this.parameters = parameterDescriptors;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return this;
        }

        ILanguageModel languageModel;
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
        /// Extracts data based on the parameters provided from the given text asynchronously
        /// </summary>
        /// <param name="text">Input text</param>
        /// <returns>A dictionary of parameter name and corresponding values.</returns>
        public async Task<Dictionary<string, string>> ExtractDataAsync(string text)
        {
            //1. Check the text size if more than 3000 characters, then split in chunks of 2000 characters with 10% overlap.
            //2. Search 2 relevant text blocks for each parameter using semantic search.
            //3. Combine the search results into a single text block
            //4. Use language model to extract the parameters
            var results = new Dictionary<string, string>();
            if (text.Length > ChunkingThreshold)
            {
                var searchtoool = SearchTool.ForSemanticSearchFromSource(text, ChunkSize, ChunkOverlap).WithMaxResultCount(2);
                foreach (var item in parameters) 
                {
                    var r = await searchtoool.SearchAsync($"{item.Name}:{item.Description}", string.Empty);

                    var txt = string.Join("\n", r.Select(x => x.content));

                    var func = new FunctionDescriptor(Name, "Extracts parameters", Enumerable.Repeat(item, 1));

                    var chatmsg = FormatPrompt(extratorPrompt, txt);
                    var response = await languageModel.GetResponseAsync(new[] { chatmsg }, new[] { func }, temperature);
                    if(response.Type == ResponseType.FunctionCall)
                    {
                        var arguments = GetArgumentValues(response);
                        object value;
                        if(arguments.TryGetValue(item.Name, out value))
                        {
                            results.Add(item.Name, value.ToString());
                        }
                    }
                }
            }
            else
            {
                var func = new FunctionDescriptor(Name, "Extracts parameters", parameters);

                var chatmsg = FormatPrompt(extratorPrompt, text);
                var response = await LanguageModel.GetResponseAsync(new[] { chatmsg }, new[] { func }, temperature);
                if (response.Type == ResponseType.FunctionCall)
                {
                    var arguments = GetArgumentValues(response);
                    foreach (var p in parameters)
                    {
                        object value;
                        if (arguments.TryGetValue(p.Name, out value))
                        {
                            results.Add(p.Name, value.ToString());
                        }
                    }
                }
            }

            return results;
        }

        private Dictionary<string, object> GetArgumentValues(LLMResponse response)
        {
            if (response.Type != ResponseType.FunctionCall) return null;

            var serializer = new JavaScriptSerializer();
            var function_call = serializer.Deserialize<Dictionary<string, object>>(response.Response);
            string args = (string)function_call["arguments"];

            var arguments = serializer.Deserialize<Dictionary<string, object>>(args);

            return arguments;
        }

        protected override async Task<Result> ExecuteCoreAsync(ExecutionContext context)
        {
            var result = new Result();

            var text = (string)context[Descriptor.InputParameters.First()];

            var dict = await ExtractDataAsync(text);

            if(dict != null && dict.Any()) {
                result.output = dict;
                result.success = true;
            }

            return result;
        }

        protected override FunctionDescriptor GetDescriptor()
        {
            if(string.IsNullOrWhiteSpace(Description))
            {
                var names = string.Join(", ", parameters.Select(p => p.Name));
                Description = $"Extracts parameters: '{names}' from the input text.";
            }
            var input = new ParameterDescriptor() { Name = "input", Description = "Raw text from where parameters needs to be extracted." };
            var function = new FunctionDescriptor(Name, Description, new[] { input });

            return function;
        }

        private ChatMessage FormatPrompt(PromptTemplate prompt, string input)
        {
            var ctx = new ExecutionContext();
            ctx[prompt.Variables.First()] = input;
            return prompt.FormatMessage(ctx);
        }
    }
}
