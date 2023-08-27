using Automation.GenerativeAI.UX.Core;
using Automation.GenerativeAI.UX.Services;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Automation.GenerativeAI.UX.ViewModels
{
    internal class LoginViewModel : ObservableObject
    {
        private const int MAX_IMAGE_WIDTH = 300;
        private const int MAX_IMAGE_HEIGHT = 300;

        private User user = new User { DisplayName = "", ID = "", Profile = string.Empty };

        public LoginViewModel()
        {
            var service = ServiceContainer.Resolve<ILoginService>();
            if(service != null)
            {
                var u = service.GetCurrentUserAsync().Result;
                if(u != null)
                {
                    user = u;
                    UserRegistered = true;
                }
            }
        }

        private bool registered = false;
        public bool UserRegistered
        {
            get { return registered; }
            set { registered = value; OnPropertyChanged(); }
        }

        public string UserName
        {
            get { return user.ID; }
            set
            {
                user.ID = value;
                OnPropertyChanged();
            }
        }

        public string DisplayName
        {
            get { return user.DisplayName; }
            set
            {
                user.DisplayName = value;
                OnPropertyChanged();
            }
        }

        private string password = null;
        public string Password
        {
            get { return password; }
            set
            {
                password = value; OnPropertyChanged();
            }
        }

        public bool PasswordRequired => ServiceContainer.Resolve<ILoginService>() != null;

        public string ProfilePic
        {
            get { return user.Profile; }
            set
            {
                user.Profile = value;
                OnPropertyChanged();
            }
        }

        private bool isloggedin = false;
        public bool IsLoggedIn
        {
            get { return isloggedin; }
            set { isloggedin = value; OnPropertyChanged(); }
        }

        #region Login Command
        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand =
                    new RelayCommandAsync(o => Login(), (o) => CanLogin()));
            }
        }

        private async Task<bool> Login()
        {
            try
            {
                var service = ServiceContainer.Resolve<ILoginService>();
                if(service != null) 
                {
                    if (string.IsNullOrEmpty(user.DisplayName))
                    {
                        user.DisplayName = user.ID;
                    }

                    if (!UserRegistered)
                    {
                        await service.RegisterUserAsync(user, password);
                    }
                    IsLoggedIn = await service.LoginAsync(user, Password);
                }
                else
                {
                    //Else do nothing
                    IsLoggedIn = true;
                }
                if(IsLoggedIn)
                {
                    ServiceContainer.Register(user);
                }
                return IsLoggedIn;
            }
            catch (Exception) { return false; }
        }

        private bool CanLogin()
        {
            if(PasswordRequired)
            {
                return !string.IsNullOrEmpty(UserName) && UserName.Length >= 2 && !string.IsNullOrEmpty(Password) && Password.Length >= 2;
            }

            return !string.IsNullOrEmpty(UserName) && UserName.Length >= 2;
        }
        #endregion

        #region Select Profile Picture Command
        private ICommand _selectProfilePicCommand;
        public ICommand SelectProfilePicCommand
        {
            get
            {
                return _selectProfilePicCommand ?? (_selectProfilePicCommand =
                    new RelayCommandAsync((o) => SelectProfilePic()));
            }
        }

        private async Task SelectProfilePic()
        {
            var dialogService = ServiceContainer.Resolve<IDialogService>();
            var pic = dialogService.OpenFile("Select image file", "Images (*.jpg;*.png)|*.jpg;*.png");
            if (!string.IsNullOrEmpty(pic))
            {
                var img = Image.FromFile(pic);
                if (img.Width > MAX_IMAGE_WIDTH || img.Height > MAX_IMAGE_HEIGHT)
                {
                    dialogService.ShowNotification($"Image size should be {MAX_IMAGE_WIDTH} x {MAX_IMAGE_HEIGHT} or less.");
                    return;
                }
                ProfilePic = pic;
                var loginService = ServiceContainer.Resolve<ILoginService>();
                if (loginService != null)
                {
                    await loginService.UpdateUserProfileAsync(user);
                }
            }
        }
        #endregion
    }
}
