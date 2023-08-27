using System.Windows;
using System.Windows.Controls;

namespace Automation.GenerativeAI.UX.Views
{
    /// <summary>
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ChatView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Title of the Chat Messages
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ChatView), new PropertyMetadata("Chat Session"));

        /// <summary>
        /// Flag to enable multiple chat sessions
        /// </summary>
        public bool MultiChatSession
        {
            get { return (bool)GetValue(MultiChatSessionProperty); }
            set { SetValue(MultiChatSessionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MultiChatSession.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MultiChatSessionProperty =
            DependencyProperty.Register("MultiChatSession", typeof(bool), typeof(ChatView), new PropertyMetadata(false));


        /// <summary>
        /// Flag to enable save button
        /// </summary>
        public bool EnableSave
        {
            get { return (bool)GetValue(EnableSaveProperty); }
            set { SetValue(EnableSaveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableSave.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableSaveProperty =
            DependencyProperty.Register("EnableSave", typeof(bool), typeof(ChatView), new PropertyMetadata(false));


        /// <summary>
        /// Flag to enable config button
        /// </summary>
        public bool EnableConfig
        {
            get { return (bool)GetValue(EnableConfigProperty); }
            set { SetValue(EnableConfigProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableConfig.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableConfigProperty =
            DependencyProperty.Register("EnableConfig", typeof(bool), typeof(ChatView), new PropertyMetadata(false));


        /// <summary>
        /// Flag to enable Chat Reset button
        /// </summary>
        public bool EnableReset
        {
            get { return (bool)GetValue(EnableResetProperty); }
            set { SetValue(EnableResetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableReset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty EnableResetProperty =
            DependencyProperty.Register("EnableReset", typeof(bool), typeof(ChatView), new PropertyMetadata(true));


        /// <summary>
        /// Register a custom routed event using the Bubble routing strategy.
        /// </summary>
        public static readonly RoutedEvent ButtonClickEvent = EventManager.RegisterRoutedEvent(
            name: "ButtonClick",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(ChatView));

        /// <summary>
        /// ButtonClick event on ChatView control
        /// </summary>
        public event RoutedEventHandler ButtonClick
        {
            add { AddHandler(ButtonClickEvent, value); }
            remove { RemoveHandler(ButtonClickEvent, value); }
        }

        private void ToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ButtonClickEvent, e.Source);

            //Raise event that will bubble up through the element tree
            RaiseEvent(routedEventArgs);
        }
    }
}
