using Automation.GenerativeAI.UX.ViewModels;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Automation.GenerativeAI.UX.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        private LoginViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginView()
        {
            InitializeComponent();
            viewModel = DataContext as LoginViewModel;
            if(viewModel != null )
            {
                viewModel.PropertyChanged += LoginViewModel_PropertyChanged;
            }
        }

        public event EventHandler LoginComplete;

        private void LoginViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsLoggedIn")
            {
                if(viewModel.IsLoggedIn)
                {
                    LoginComplete?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void PasswordTxtBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(this.DataContext != null)
            {
                ((LoginViewModel)this.DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
