namespace CloudBox.Photo.Views;

public partial class AppTitleView : ContentView
{
	public AppTitleView()
	{
		InitializeComponent();
	}

    private void ImageButton_Clicked(object sender, EventArgs e)
    {
		Shell.Current.GoToAsync("LoginPage");
		//Shell.Current.Navigation.RemovePage()
    }
}