using Automation.GenerativeAI.UX.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.UX.Services
{
    /// <summary>
    /// Implements ChatService for sending and receiving messages
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Creates a new ChatSession asynchronously
        /// </summary>
        /// <param name="id">Unique Id for the session</param>
        Task CreateChatSessionAsync(string id);

        /// <summary>
        /// Sends message
        /// </summary>
        /// <param name="id">Chat session id to which message to be sent</param>
        /// <param name="message">Message</param>
        /// <returns>Response by the chat agent</returns>
        Task<string> SendMessageAsync(string id, string message);

        /// <summary>
        /// Gets chat history of the given session.
        /// </summary>
        /// <param name="id">Chat session id</param>
        /// <returns>List of chat messages</returns>
        Task<IEnumerable<ChatMessage>> GetChatHistoryAsync(string id);

        /// <summary>
        /// Updates the chat history of the given session.
        /// </summary>
        /// <param name="id">Chat session id</param>
        /// <param name="messages">List of chat messages</param>
        Task UpadateChatHistoryAsync(string id, IEnumerable<ChatMessage> messages);
    }
}
