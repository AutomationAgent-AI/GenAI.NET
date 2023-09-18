using Automation.GenerativeAI.Chat;
using Automation.GenerativeAI.Interfaces;
using Automation.GenerativeAI.Tools;
using Automation.GenerativeAI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Automation.GenerativeAI.Agents
{
    /// <summary>
    /// Represents a type of Agent
    /// </summary>
    public enum AgentType
    {
        /// <summary>
        /// Represents an agent that makes use of OpenAI Function call capability.
        /// </summary>
        FunctionAgent
    }

    /// <summary>
    /// Represents an Agent that can perform certain actions to accomplish given objective.
    /// </summary>
    public abstract class Agent
    {
        protected List<ChatMessage> Messages = new List<ChatMessage>();
        protected ToolsCollection Tools;
        protected string SystemPrompt;
        protected double Temperature = 0.8;
        private ILanguageModel languageModel;
        private int MaxAllowedSteps = 10;
        private List<AgentAction> Steps = new List<AgentAction>();

        /// <summary>
        /// Name of the Agent
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Description of the agent
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Sets description
        /// </summary>
        /// <param name="description">Description of the agent.</param>
        /// <returns>This Agent</returns>
        public Agent WithDescription(string description)
        {
            this.Description = description;
            return this;
        }

        /// <summary>
        /// Sets a list of allowed tools for agent to use.
        /// </summary>
        /// <param name="tools">List of tools</param>
        /// <returns>This Agent</returns>
        public Agent WithTools(IEnumerable<IFunctionTool> tools)
        {
            Tools = new ToolsCollection(tools);
            return this;
        }

        /// <summary>
        /// Sets language model to the agent
        /// </summary>
        /// <param name="languageModel">Language model for agent to perform certain tasks</param>
        /// <returns>This Agent</returns>
        public Agent WithLanguageModel(ILanguageModel languageModel)
        {
            this.languageModel = languageModel;
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter to agent to define the creativity.
        /// </summary>
        /// <param name="temperature">A value between 0 and 1 to define creativity</param>
        /// <returns>This Agent</returns>
        public Agent WithTemperature(double temperature)
        {
            Temperature = temperature;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of steps this agent can execute.
        /// </summary>
        /// <param name="maxAllowedSteps">Maximum number of steps that can be executed.</param>
        /// <returns>This Agent</returns>
        public Agent WithMaxAllowedSteps(int maxAllowedSteps = 10)
        {
            MaxAllowedSteps = maxAllowedSteps;
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

        /// <summary>
        /// Provides a next agent action based on the given message history.
        /// </summary>
        /// <param name="messages">History of messages as a list</param>
        /// <returns>AgentAction</returns>
        protected abstract Task<AgentAction> GetNextActionAsync(List<ChatMessage> messages);

        /// <summary>
        /// Executes the given objective
        /// </summary>
        /// <param name="objective">The detailed objective for agent to achieve.</param>
        /// <returns>FinishAction if objective is met or got an error. It may return AgentAction if the
        /// tool associated with the action can't be executed. It provides clients to execute the
        /// action logic and then call UpdateToolResponseAsync to proceed further with execution.</returns>
        public async Task<AgentAction> ExecuteAsync(string objective)
        {
            Messages.Clear(); //Clear the old message before starting on new objective.
            Steps.Clear(); //Clear old steps as we are starting a new objective.

            Messages.Add(new ChatMessage(Role.system, SystemPrompt));
            Messages.Add(new ChatMessage(Role.user, objective));
            var action = await ExecuteCoreAsync();
            return action;
        }

        private async Task<AgentAction> ExecuteCoreAsync()
        {
            AgentAction action = null;
            FinishAction done = null;

            while (done == null)
            {
                action = await GetNextActionAsync(Messages);
                Steps.Add(action);

                done = action as FinishAction;
                if (done != null) return action;

                if (Steps.Count > MaxAllowedSteps)
                {
                    var step = new StepAction() { tool = action.Tool.Name, parameters = new Dictionary<string, object>(action.ExecutionContext.GetParameters()) };
                    return new FinishAction(FunctionTool.ToJsonString(step), $"ERROR: Exceeded the maximum allowed steps, MaxAllowedSteps ={MaxAllowedSteps}");
                }

                if (action != null)
                {
                    var result = await action.ExecuteAsync();
                    //Action can't be executed, return this action to caller so that the caller can execute
                    //based on its execution logic.
                    if (string.IsNullOrEmpty(result)) return action;
                    Messages.Add(new FunctionMessage(action.Tool.Name, result));
                }
            }

            return action;
        }

        /// <summary>
        /// Called by the client to update the tool's execution result with the agent, if the tool
        /// corresponding to the agent action was executed by client.
        /// </summary>
        /// <param name="toolName">Name of the tool that executed</param>
        /// <param name="output">Output of the tool.</param>
        /// <returns>FinishAction if objective is met or got an error. AgentAction if it is
        /// required to be executed by client.</returns>
        public async Task<AgentAction> UpdateAgentActionResponseAsync(string toolName, string output)
        {
            var msg = new FunctionMessage(toolName, output);
            Messages.Add(msg);
            return await ExecuteCoreAsync();
        }

        /// <summary>
        /// The derived class implements this method to provide a system prompt text for the language model.
        /// </summary>
        /// <param name="username">Name of the user interactive with agent</param>
        /// <param name="date">Today's date</param>
        /// <param name="workingdir">Full path of the working directory</param>
        /// <returns>System prompt text</returns>
        protected abstract string LoadSystemPrompt(string username, string date, string workingdir);

        /// <summary>
        /// Creates an agent
        /// </summary>
        /// <param name="name">A unique name of the agent</param>
        /// <param name="workingdir">Full path of the working directory</param>
        /// <param name="type">Type of the agent to create</param>
        /// <returns>Agent</returns>
        public static Agent Create(string name, string workingdir = "", AgentType type = AgentType.FunctionAgent) 
        {
            Agent agent = null;
            if(string.IsNullOrEmpty(workingdir)) { workingdir = Environment.CurrentDirectory; }

            if(type == AgentType.FunctionAgent)
            {
                agent = new FunctionAgent() { Name = name };
            }

            agent.SystemPrompt = agent.LoadSystemPrompt(
                Environment.UserName, 
                System.DateTime.Today.ToShortDateString(), 
                workingdir);

            return agent;
        }

        /// <summary>
        /// Converts the LLMResponse to ChatMessage
        /// </summary>
        /// <param name="response">A response returned from the language model.</param>
        /// <returns>ChatMessage</returns>
        internal protected static ChatMessage MessgeFromResponse(LLMResponse response)
        {
            switch (response.Type)
            {
                case ResponseType.Failed:
                    break;
                case ResponseType.Done:
                    return new ChatMessage(Role.assistant, response.Response);
                case ResponseType.Partial:
                    return new ChatMessage(Role.assistant, response.Response);
                case ResponseType.FunctionCall:
                    var serializer = new JavaScriptSerializer();
                    var function_call = serializer.Deserialize<Dictionary<string, object>>(response.Response);
                    return new FunctionCallMessage() { function_call = function_call };
                default:
                    break;
            }
            return null;
        }
    }

    internal class FunctionAgent : Agent
    {
        private List<ChatMessage> messages = new List<ChatMessage>();
        public FunctionAgent()
        {

        }

        protected override async Task<AgentAction> GetNextActionAsync(List<ChatMessage> messages)
        {
            var functions = Enumerable.Empty<FunctionDescriptor>();
            if (Tools != null)
            {
                functions = Tools.GetFunctions();
            }

            LLMResponse response = await LanguageModel.GetResponseAsync(messages, functions, Temperature);
            if (response.Type == ResponseType.Failed)
            {
                return new FinishAction(string.Empty, response.Response);
            }

            if(response.Type == ResponseType.Done)
            {
                return new FinishAction(response.Response);
            }
            var msg = MessgeFromResponse(response);
            if (Tools != null && response.Type == ResponseType.FunctionCall)
            {
                Messages.Add(msg);
                var fmsg = msg as FunctionCallMessage;
                object function = null;
                fmsg.function_call.TryGetValue("name", out function);
                string args = (string)fmsg.function_call["arguments"];

                var serializer = new JavaScriptSerializer();
                var arguments = serializer.Deserialize<Dictionary<string, object>>(args);
                var tool = Tools.GetTool((string)function);
                return new AgentAction(tool, new ExecutionContext(arguments), $"Need to execute {tool.Name}");
            }

            return new FinishAction(string.Empty, "Error on execution!!");
        }

        private async Task<AgentAction> ObsoleteExecuteAsync(string objective)
        {
            if(messages.Count == 0)
            {
                messages.Add(new ChatMessage(Role.system, SystemPrompt));
                messages.Add(new ChatMessage(Role.user, objective));
            }

            var functions = Enumerable.Empty<FunctionDescriptor>();
            if (Tools != null)
            {
                functions = Tools.GetFunctions();
            }

            LLMResponse response = new LLMResponse() { Type = ResponseType.Failed };
            while (response.Type != ResponseType.Done)
            {
                response = await LanguageModel.GetResponseAsync(messages, functions, Temperature);
                //When there is no tool registered then one call is good
                if (response.Type == ResponseType.Failed) return new FinishAction(string.Empty, response.Response);

                var msg = MessgeFromResponse(response);
                messages.Add(msg);

                if (Tools != null && response.Type == ResponseType.FunctionCall)
                {
                    //Try to execute the message
                    var fmsg = msg as FunctionCallMessage;
                    object function = null;
                    fmsg.function_call.TryGetValue("name", out function);
                    string args = (string)fmsg.function_call["arguments"];

                    var serializer = new JavaScriptSerializer();
                    var arguments = serializer.Deserialize<Dictionary<string, object>>(args);
                    var context = new ExecutionContext(arguments);
                    var output = await Tools.ExecuteAsync((string)function, context);

                    if (string.IsNullOrEmpty(output))
                    {
                        messages.Add(fmsg);
                        var tool = Tools.GetTool((string)function);
                        return new AgentAction(tool, new ExecutionContext(arguments), $"Need to execute {tool.Name}");
                    }

                    messages.Add(new FunctionMessage((string)function, output));
                }
            }
            if (response.Type == ResponseType.Done)
            {
                return new FinishAction(response.Response);
            }
            return new FinishAction(string.Empty, "Error on execution!!");
        }

        protected override string LoadSystemPrompt(string username, string date, string workingdir)
        {
            var template = EmbeddedResource.GetrResource("Automation.GenerativeAI.Prompts.FunctionAgentPrompt.txt");
            var prompt = new PromptTemplate(template, Role.system);
            var ctx = new ExecutionContext();
            ctx[prompt.Variables[0]] = workingdir;
            ctx[prompt.Variables[1]] = date;
            ctx[prompt.Variables[2]] = username;

            return prompt.FormatMessage(ctx).content;
        }
    }
}
