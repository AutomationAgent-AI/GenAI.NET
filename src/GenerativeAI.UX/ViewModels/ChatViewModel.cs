using Automation.GenerativeAI.UX.Core;
using Automation.GenerativeAI.UX.Models;
using Automation.GenerativeAI.UX.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Automation.GenerativeAI.UX.ViewModels
{
    internal class ChatViewModel : ObservableObject
    {
        public ObservableCollection<ChatSession> Sessions { get; set; }

        private ChatSession currentsession;
        public ChatSession CurrentSession
        {
            get { return currentsession; }
            set
            {
                if(currentsession != null)
                {
                    currentsession.MessageReceived -= Currentsession_MessageReceived;
                }
                currentsession = value;
                if(currentsession != null)
                {
                    Messages = new NotifyTaskCompletion<ObservableCollection<ChatMessage>>(GetSessionMessages(currentsession));
                    currentsession.MessageReceived += Currentsession_MessageReceived;
                }
                OnPropertyChanged();
            }
        }

        private async Task<ObservableCollection<ChatMessage>> GetSessionMessages(ChatSession session)
        {
            messages.Clear();
            var msgs = await session.GetMessagesAsync();
            messages = new ObservableCollection<ChatMessage>(msgs);
            return messages;
        }

        private void Currentsession_MessageReceived(object sender, IEnumerable<ChatMessage> e)
        {
            Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
            {
                foreach (ChatMessage message in e)
                {
                    messages.Add(message);
                }
                Messages = new NotifyTaskCompletion<ObservableCollection<ChatMessage>>(Task.FromResult(messages));
            }));
        }

        public string UserName
        {
            get
            {
                var user = ServiceContainer.Resolve<User>();
                if (user != null) { return user.ID; }

                return "";
            }
        }

        public string ProfilePic
        {
            get 
            {
                var user = ServiceContainer.Resolve<User>();
                if(user != null) { return user.Profile; }

                return ""; 
            }
        }

        public string UserDisplayName
        {
            get
            {
                var user = ServiceContainer.Resolve<User>();
                if(user != null) return user.DisplayName;

                return "";
            }
        }

        public ICommand SendCommand { get; set; }

        public ICommand CreateNewChatSession { get; set; }
        public ICommand DeleteChatSession { get; set; }
        public ICommand RenameChatSession { get; set; }
        public ICommand ResetChatCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ConfigCommand { get; set; }

        ObservableCollection<ChatMessage> messages = new ObservableCollection<ChatMessage>();
        NotifyTaskCompletion<ObservableCollection<ChatMessage>> msgTask;

        public NotifyTaskCompletion<ObservableCollection<ChatMessage>> Messages 
        {
            get => msgTask; 
            private set
            {
                msgTask = value;
                OnPropertyChanged();
            }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                message = value;
                OnPropertyChanged();
            }
        }

        public ChatViewModel()
        {
            Sessions = new ObservableCollection<ChatSession>();
            Messages = new NotifyTaskCompletion<ObservableCollection<ChatMessage>>(Task.FromResult(messages));

            SendCommand = new RelayCommandAsync(o => SendMessage(message));

            CreateNewChatSession = new RelayCommandAsync(o => CreateSession());

            DeleteChatSession = new RelayCommand(o => DeleteSession((string)o), o => Sessions.Count > 1);

            ResetChatCommand = new RelayCommandAsync(o => ResetChatSession(CurrentSession));
            SaveCommand = new RelayCommand(o => ShowMessageBox("Save"));
            ConfigCommand = new RelayCommand(o => ShowMessageBox("Configure"));

            CreateNewChatSession.Execute(this); //initialize the first session
        }

        private async Task ResetChatSession(ChatSession session)
        {
            messages.Clear();
            Messages = new NotifyTaskCompletion<ObservableCollection<ChatMessage>>(Task.FromResult(messages));
            await session.ResetSessionAsync();
        }

        private async Task SendMessage(string message)
        {
            var msg = new ChatMessage
            {
                Message = message,
                Role = Role.User,
                Time = DateTime.Now,
            };
            messages.Add(msg);
            Message = "";
            await CurrentSession.AddMessageAsync(msg);
        }

        public bool CanDeleteSession => Sessions.Count > 1;

        private void DeleteSession(string name)
        {
            var session = Sessions.FirstOrDefault(x => x.Name == name);
            Sessions.Remove(session);
            OnPropertyChanged("CanDeleteSession");
        }

        private async Task CreateSession()
        {
            CurrentSession = new ChatSession()
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"Chat Session {Sessions.Count}",
                Time = DateTime.Now,
            };

            Sessions.Add(CurrentSession);
            OnPropertyChanged("CanDeleteSession");

            var service = ServiceContainer.Resolve<IChatService>();
            if(service != null)
            {
                await service.CreateChatSessionAsync(CurrentSession.Id);
            }
        }

        private void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }
    }
}
