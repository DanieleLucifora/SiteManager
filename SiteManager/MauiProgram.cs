using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using SiteManager.Models;
using SiteManager.Services;

namespace SiteManager;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        // Caricamento iniziale dei dati dal database
        List<Cantiere> cantieri = CantiereService.OttieniCantieri().ToList();
        List<Operaio> operai = OperaioService.OttieniOperai().ToList();
        List<Tasks> tasks = new List<Tasks>();
        List<Materiale> materiali = MaterialeService.OttieniMateriali().ToList();

        // Ottiene i tasks per ciascun cantiere
        foreach (var cantiere in cantieri)
        {
            tasks.AddRange(TasksService.OttieniTasks(cantiere));
        }

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		#if DEBUG
				builder.Logging.AddDebug();
		#endif

		return builder.Build();
	}

}