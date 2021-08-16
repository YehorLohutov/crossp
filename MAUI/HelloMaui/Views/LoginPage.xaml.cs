using Microsoft.Maui.Controls;
using MAUI.ViewModels;

namespace MAUI.Views
{
    public partial class LoginPage : ContentPage
	{
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel();
		}
    }
}
