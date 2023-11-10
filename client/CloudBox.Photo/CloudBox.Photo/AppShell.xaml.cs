using CloudBox.Photo.Pages;

namespace CloudBox.Photo;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute("LoginPage", typeof(LoginPage));
        Routing.RegisterRoute("AppShell", typeof(AppShell));
        Routing.RegisterRoute("PhotoPage", typeof(PhotoPage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("ViewPhotoPage", typeof(ViewPhotoPage));


    }
}
