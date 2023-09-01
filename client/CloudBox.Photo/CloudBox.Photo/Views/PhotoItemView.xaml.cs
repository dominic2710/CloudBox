using CloudBox.Photo.Helpers;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace CloudBox.Photo.Views;

public partial class PhotoItemView : ContentView
{ 
    public PhotoItemView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(PhotoItemView), string.Empty, propertyChanged: OnUrlPropertyChanged);
    public string Url
    {
        get => (string)GetValue(UrlProperty);
        set
        {
            SetValue(UrlProperty, value);
        }
    }

    async Task GetPhoto()
    {
        if (String.IsNullOrEmpty(Url)) return;

        //lblUrl.Text = Url;
        var imageBytes = await Services.ServiceProvider.GetInstance().GetByteArrayAsync(Url);
        if (imageBytes != null)
        {
            var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            
            await Dispatcher.DispatchAsync(() =>
            {
                //lblUrl.Text = Url;
                imgMain.Source = imageSource;
            });
        }
        else
        {
            imgMain.Source = Global.IMAGE_LOAD_FAILED;
        }
    }

    private void ContentView_Loaded(object sender, EventArgs e)
    {
        imgMain.Source = Global.IMAGE_LOADING;
        Task.Run(async () =>
        {
            await GetPhoto();
        });
    }

    private static async void OnUrlPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PhotoItemView photoItemView && newValue is string newUrl)
        {
            if (newUrl == photoItemView.Url) return;

            photoItemView.Url = newUrl; // Set the Url property

            // Perform your action here
            await photoItemView.GetPhoto();
        }
    }
}