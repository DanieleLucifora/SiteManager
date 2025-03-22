using Microsoft.Extensions.Logging;
using Microcharts.Maui;

namespace SiteManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			// Registra Microcharts.Maui
            .UseMicrocharts();

		#if DEBUG
						builder.Logging.AddDebug();
		#endif

		return builder.Build();
	}

}