using System;

namespace Automation.GenerativeAI.UX.Models
{
    /// <summary>
    /// Sender's role for the chat message
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Sent by user
        /// </summary>
        User,

        /// <summary>
        /// Sent by Assistant
        /// </summary>
        Assistant
    }

    /// <summary>
    /// ChatMessage object to be rendered in ChatView
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Message content
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Role of the sender of this message
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Timestamp when the message was sent
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// If the message was native origin, i.e sent by user
        /// </summary>
        public bool IsOriginNative => Role == Role.User;
    }
}
