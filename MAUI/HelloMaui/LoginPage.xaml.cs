using System;
using Microsoft.Maui.Controls;

namespace MAUI
{
    public partial class LoginPage : ContentPage
	{
        public LoginPage()
        {
            InitializeComponent();
		}

        protected void OnLoginButtonClick(object sender, EventArgs e)
        {
            
            DisplayAlert($"Title", $"{NameEntry.Text} {PasswordEntry.Text}", $"Cancel!");
        }
    }
}
