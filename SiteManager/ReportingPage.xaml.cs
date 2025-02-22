using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SiteManager;

public partial class ReportingPage : ContentPage
{
	private readonly CantiereService _cantiereService;
	public ObservableCollection<Cantiere> CantieriList { get; set; }

	public ReportingPage()
	{
		InitializeComponent();
        _cantiereService = new CantiereService();
        CantieriList = new ObservableCollection<Cantiere>();
        CantieriCollectionView.ItemsSource = CantieriList;
		LoadCantieri();
	}

    private void LoadCantieri()
    {
        var cantieri = CantiereService.OttieniCantieri();
        foreach (var cantiere in cantieri)
        {
            CantieriList.Add(cantiere);
        }        
        CantieriCollectionView.ItemsSource = CantieriList;
    }

	private async void GeneraReport_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
		{
			bool conferma = await DisplayAlert("Conferma", $"Vuoi generare il report del cantiere di {selectedCantiere.Città}?", "Si", "No");
			if (conferma)
			{
				// Percorso relativo per lo script Python
				var scriptPath = "Report/report_generator.py";
				// Percorso assoluto per l'interprete Python (assicurati che sia corretto su tutte le macchine)
				var pythonPath = "/usr/bin/python3";
				var processStartInfo = new ProcessStartInfo
				{
					FileName = pythonPath,
					Arguments = scriptPath,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				using (var process = Process.Start(processStartInfo))
				{
					using (var reader = process.StandardOutput)
					{
						string result = reader.ReadToEnd();
						if (File.Exists(result.Trim()))
						{
							await DisplayAlert("Report Generato", $"Il report è stato generato con successo: {result}", "OK");
						}
						else
						{
							await DisplayAlert("Errore", $"Errore nella generazione del report: {result}", "OK");
						}
					}
				}				
			}
		}

	}
}