using MauiIcons.Fluent;
using MauiIcons.Material;

namespace CloudBox.Photo;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiApp<App>().UseFluentMauiIcons()
            .UseMauiApp<App>().UseMaterialMauiIcons()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "IconFontTypes");
            });

		return builder.Build();
	}
}
