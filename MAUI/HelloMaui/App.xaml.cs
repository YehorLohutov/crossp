using Microsoft.Maui;
using Microsoft.Maui.Controls;

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
