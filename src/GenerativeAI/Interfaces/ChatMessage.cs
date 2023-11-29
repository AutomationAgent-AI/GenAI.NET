using System.Collections.Generic;

namespace Automation.GenerativeAI.Interfaces
{
    /// <summary>
    /// Represents various roles involved in the conversation.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// User usually asks questions in the conversation.
        /// </summary>
        user,

        /// <summary>
        /// Assistant is AI BOT that responds to the questions of user.
        /// </summary>
        assistant,

        /// <summary>
        /// System user provides instruction to BOT on how to respond to certain questions.
        /// </summary>
        system,

        /// <summary>
        /// A function user provides the response returned by a function call 
        /// identified by the language model.
        /// </summary>
        function
    }

    /// <summary>
    /// Represents a chat message
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Creates ChatMessage object with given Role and content
        /// </summary>
        /// <param name="role"></param>
        /// <param name="content"></param>
        public ChatMessage(Role role, string content)
        {
            this.role = role.ToString();
            this.content = content;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ChatMessage() { }

        /// <summary>
        /// Role of the messenger
        /// </summary>
        public string role { get; set; }

        /// <summary>
        /// Message content
        /// </summary>
        public string content { get; set; }
    }

    /// <summary>
    /// Represents a function message to send the function call output to the 
    /// language model to summarize the result in natural language.
    /// </summary>
    public class FunctionMessage : ChatMessage
    {
        /// <summary>
        /// Create a function message
        /// </summary>
        /// <param name="name">Name of the function that was executed</param>
        /// <param name="output">Output values returned by the function</param>
        public FunctionMessage(string name, string output) : base(Role.function, output)
        {
            this.name = name;
        }

        /// <summary>
        /// Name of the function that was called
        /// </summary>
        public string name { get; set; }
    }

    /// <summary>
    /// Represents a function call message usually returned by the language model.
    /// </summary>
    public class FunctionCallMessage : ChatMessage
    {
        /// <summary>
        /// Creates a FunctionCallMessage
        /// </summary>
        public FunctionCallMessage() : base(Role.assistant, null)
        {
        }

        /// <summary>
        /// Function call details that contains function name and arguments of 
        /// the function as returned by language model. For example, 
        /// { "name": "get_current_weather",
        /// "arguments": "{ \"location\": \"Boston, MA\"}" }
        /// </summary>
        public Dictionary<string, object> function_call { get; set; }
    }

    /// <summary>
    /// Provides the type of response received from the language model
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Request failed
        /// </summary>
        Failed,

        /// <summary>
        /// Request is processed successfully
        /// </summary>
        Done,

        /// <summary>
        /// Request is partially complete
        /// </summary>
        Partial,

        /// <summary>
        /// Request ended with function call details
        /// </summary>
        FunctionCall
    }

    /// <summary>
    /// Response returned by the large language model for a give message
    /// </summary>
    public class LLMResponse
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public LLMResponse() { }

        /// <summary>
        /// Response returned by the Language Model
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Response type returned by the language model
        /// </summary>
        public ResponseType Type { get; set; }
    }
}
