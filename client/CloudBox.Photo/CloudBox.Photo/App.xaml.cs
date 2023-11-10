using CloudBox.Photo.Pages;

namespace CloudBox.Photo;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new LoginPage();
	}
}
