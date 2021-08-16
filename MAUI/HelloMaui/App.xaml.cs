using Microsoft.Maui;
using Microsoft.Maui.Controls;
using MAUI.Views;

namespace MAUI
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new LoginPage();
		}
	}
}
