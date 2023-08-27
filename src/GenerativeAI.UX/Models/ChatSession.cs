using Automation.GenerativeAI.UX.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Automation.GenerativeAI.UX.Models
{
    internal class ChatSession
    {
        public event EventHandler<IEnumerable<ChatMessage>> MessageReceived;

        /// <summary>
        /// Session Id as maintained by ChatService
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Name of the session as shown in the UI
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Time when the session was created
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Gets list of messages
        /// </summary>
        public IEnumerable<ChatMessage> Messages => messages;

        private List<ChatMessage> messages = null;

        private void NotifyMessageReceived(IEnumerable<ChatMessage> chatMessages)
        {
            if (MessageReceived != null) MessageReceived(this, chatMessages);
        }

        /// <summary>
        /// List of Chat messages sent to this specific session. Messages can be from user or the AI assistant
        /// </summary>
        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync()
        {
            if(messages == null)
            {
                messages = new List<ChatMessage>();
                var service = ServiceContainer.Resolve<IChatService>();
                if(service == null)
                {
                    return messages;
                }

                var msgs = await service.GetChatHistoryAsync(Id);
                messages.AddRange(msgs);
                NotifyMessageReceived(msgs);
            }

            return messages;
        }


        /// <summary>
        /// Adds a chat message to the current session asynchronously. It sends this message to
        /// the Chat service to get a response. If a response is received, it raises MessageReceived
        /// event.
        /// </summary>
        /// <param name="message">Chat message to be added to the conversation.</param>
        public async Task AddMessageAsync(ChatMessage message)
        {
            var msgs = await GetMessagesAsync();
            messages.Add(message);
            var service = ServiceContainer.Resolve<IChatService>();
            if(service != null)
            {
                var msg = await service.SendMessageAsync(this.Id, message.Message);
                var chatmsg = new ChatMessage { Message = msg, Role = Role.Assistant, Time = DateTime.Now };
                messages.Add(chatmsg);
                NotifyMessageReceived(Enumerable.Repeat(chatmsg, 1));
            }
        }

        /// <summary>
        /// Resets the current session by clearing its messages and creating
        /// a fresh chat session with the ChatService.
        /// </summary>
        public async Task ResetSessionAsync()
        {
            messages.Clear();
            var service = ServiceContainer.Resolve<IChatService>();
            if (service != null)
            {
                Id = Guid.NewGuid().ToString();
                await service.CreateChatSessionAsync(Id);
            }
        }
    }
}
