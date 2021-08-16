using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MAUI.Services;
using MAUI.Models;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace MAUI.ViewModels
{
    class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        
        public Command LoginCommand { get; set; }


        public string Login
        {
            get => loginModel.Login;
            set
            {
                if (loginModel != default)
                {
                    loginModel.Login = value;
                    Password += 's';
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Login)));
                }
            }
        }
        public string Password 
        {
            get => loginModel.Password;
            set 
            {
                if (loginModel != default)
                {
                    loginModel.Password = value;
                    PropertyChanged(this, new PropertyChangedEventArgs(nameof(Password)));
                }
            } 
        }

        private IAuthorizationService authorizationService = default;
        private LoginModel loginModel = new LoginModel();

        public LoginViewModel()
        {
            authorizationService = ServicesFactory.GetAuthorizationService();

            LoginCommand = new Command(
                async () => await LoginAsync());
            //() => Validator.TryValidateObject(loginModel, new ValidationContext(loginModel), new List<ValidationResult>()));
        }

        private async Task LoginAsync()
        {
            Token token = await authorizationService.GetTokenAsync(Login, Password);
        }
    }
}
