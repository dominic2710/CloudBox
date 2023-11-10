using CloudBox.Photo.Helpers;
using CloudBox.Photo.Services.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ServiceProvider = CloudBox.Photo.Services.ServiceProvider;

namespace CloudBox.Photo.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LoginViewModel()
        {
            UserName = "chitam2710@gmail.com";
            Password = "Abc12345";
            IsProcessing = false;

            LoginCommand = new Command(() =>
            {
                if (IsProcessing) return;

                if (UserName.Trim() == "" || Password.Trim() == "") return;

                IsProcessing = true;
                Login().GetAwaiter().OnCompleted(() =>
                {
                    IsProcessing = false;
                });
            });
        }

        async Task Login()
        {
            try
            {
                var request = new LoginRequest
                {
                    Email = UserName,
                    Password = Password,
                };
                var response = await ServiceProvider.GetInstance().Authenticate(request);
                if (response.StatusCode == 200)
                {
                    Global.LOGIN_USERID = response.Id;
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Cloud Photos", response.StatusMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Cloud Photos", ex.Message, "OK");
            }
        }

        private string userName;
        private string password;
        private bool isProcessing;

        public string UserName
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(); }
        }

        public bool IsProcessing
        {
            get { return isProcessing; }
            set { isProcessing = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
    }
}
