using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Agents
{
    /// <summary>
    /// Maintains System and User Prompts and parses the given response based on its prompt schema.
    /// </summary>
    internal interface IResponseParser
    {
        /// <summary>
        /// Given the tools it generates system prompt
        /// </summary>
        /// <param name="tools"></param>
        /// <returns>ChatMessage</returns>
        ChatMessage GetSystemPrompt(IEnumerable<IFunctionTool> tools);

        /// <summary>
        /// Generates user prompt with query and the scratchpad
        /// </summary>
        /// <param name="query"></param>
        /// <param name="scratchpad"></param>
        /// <returns>ChatMessage</returns>
        ChatMessage GetUserPrompt(string query);

        /// <summary>
        /// Parses the given response from LLM to generate AgentAction if any.
        /// </summary>
        /// <param name="response">Response from LLM</param>
        /// <returns>AgentAction based on the response</returns>
        AgentAction ParseResponse(LLMResponse response);

        /// <summary>
        /// Appends the observation string to the scratchpad.
        /// </summary>
        /// <param name="observation"></param>
        void AppendObservation(string observation);

        /// <summary>
        /// Appends the thought to the scratchpad.
        /// </summary>
        /// <param name="thought"></param>
        void AppendThought(string thought);

        /// <summary>
        /// Provides the agent's scratchpad
        /// </summary>
        string ScratchPad { get; }
    }

    class StepAction
    {
        public string tool { get; set; }
        public Dictionary<string, object> parameters { get; set; }
    }

    internal class ChainOfThoughtResponsePraser : IResponseParser
    {
        private ToolsCollection tools;
        private StringBuilder scratchPad = new StringBuilder();

        public ChatMessage GetSystemPrompt(IEnumerable<IFunctionTool> tools)
        {
            this.tools = new ToolsCollection(tools);

            var sb = new StringBuilder();
            foreach (var tool in tools)
            {
                sb.AppendLine($"{tool.Descriptor.Name}: {tool.Descriptor.Description}");
                foreach (var parameter in tool.Descriptor.Parameters.Properties)
                {
                    sb.AppendLine($"  - {parameter.Name}: {parameter.Description}");
                }
            }
            var template = EmbeddedResource.GetrResource("Automation.GenerativeAI.Prompts.ActionPlannerPrompt.txt");
            var prompt = new PromptTemplate(template, Role.system);
            var ctx = new ExecutionContext();
            ctx[prompt.Variables[0]] = sb.ToString();
            ctx[prompt.Variables[1]] = Environment.CurrentDirectory;
            ctx[prompt.Variables[2]] = DateTime.Now.ToShortDateString();
            return prompt.FormatMessage(ctx);
        }

        public ChatMessage GetUserPrompt(string query)
        {
            var prompt = $"Begin!\r\n\r\n[QUESTION]\r\n{query}\r\n{scratchPad.ToString()}";

            return new ChatMessage(Role.user, prompt);
        }

        public AgentAction ParseResponse(LLMResponse response)
        {
            Match finalAnswerMatch = s_finalAnswerRegex.Match(response.Response);
            if(finalAnswerMatch.Success) 
            {
                var output = finalAnswerMatch.Groups[1].Value.Trim();
                return new FinishAction(output); 
            }

            // Extract thought
            Match thoughtMatch = s_thoughtRegex.Match(response.Response);
            string thought = string.Empty;
            string observation = string.Empty;

            if(thoughtMatch.Success)
            {
                thought = thoughtMatch.Value.Trim();
            }
            else if (!response.Response.Contains(Action))
            {
                thought = response.Response;
            }
            else
            {
                throw new InvalidOperationException("Unexpected response format");
            }

            thought = thought.Replace(Thought, string.Empty).Trim();

            AppendThought(thought);

            // Extract action
            Match actionMatch = s_actionRegex.Match(response.Response);
            if (actionMatch.Success)
            {
                var json = actionMatch.Groups[1].Value.Trim();

                try
                {
                    var serializer = new JavaScriptSerializer();
                    var step = serializer.Deserialize<StepAction>(json);
                    if(step == null)
                    {
                        observation = $"System step parsing error, empty JSON: {json}";
                    }

                    var ctx = new ExecutionContext(step.parameters);
                    IFunctionTool tool = tools.GetTool(step.tool);

                    var action = new AgentAction(tool, ctx, thought);
                    return action;
                }
                catch (Exception ex)
                {
                    observation = $"System step parsing error, invalid JSON: {json}";
                }
            }

            if (string.IsNullOrEmpty(thought))
            {
                observation = "System step error, no thought or action found. Please give a valid thought and/or action.";
            }

            return new FinishAction(observation);
        }

        public void AppendObservation(string observation)
        {
            scratchPad.AppendLine(Observation);
            scratchPad.AppendLine(observation);
        }

        public void AppendThought(string thought)
        {
            scratchPad.AppendLine(Thought);
            scratchPad.AppendLine(thought);
        }

        /// <summary>
        /// The Action tag
        /// </summary>
        private const string Action = "[ACTION]";

        /// <summary>
        /// The Thought tag
        /// </summary>
        private const string Thought = "[THOUGHT]";

        /// <summary>
        /// The Observation tag
        /// </summary>
        private const string Observation = "[OBSERVATION]";

        /// <summary>
        /// The prefix used for the scratch pad
        /// </summary>
        private const string ScratchPadPrefix = "This was my previous work (but they haven't seen any of it! They only see what I return as final answer):";

        /// <summary>
        /// The regex for parsing the action response
        /// </summary>
        private static readonly Regex s_actionRegex = new Regex(@"\[ACTION\][^{}]*({(?:[^{}]*{[^{}]*})*[^{}]*})", RegexOptions.Singleline);

        /// <summary>
        /// The regex for parsing the thought response
        /// </summary>
        private static readonly Regex s_thoughtRegex = new Regex(@"(\[THOUGHT\])?(?<thought>.+?)(?=\[ACTION\]|$)", RegexOptions.Singleline);

        /// <summary>
        /// The regex for parsing the final answer response
        /// </summary>
        private static readonly Regex s_finalAnswerRegex = new Regex(@"\[FINAL ANSWER\](?<final_answer>.+)", RegexOptions.Singleline);

        public string ScratchPad => scratchPad.ToString();
    }
}
