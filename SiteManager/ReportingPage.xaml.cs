using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var payload = new
                    {
                        cantiere = selectedCantiere.Città  // Passiamo il nome della città al server
                    };

                    var jsonContent = new StringContent(
                        JsonConvert.SerializeObject(payload),
                        Encoding.UTF8,
                        "application/json"
                    );

                    string serverUrl = "http://localhost:5001/genera_report"; // URL API del server Flask
                    HttpResponseMessage response = await client.PostAsync(serverUrl, jsonContent);
                    string result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Report Generato", $"Il report è stato creato con successo:\n{result}", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Errore", $"Errore nella generazione del report:\n{result}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Errore", $"Si è verificato un errore:\n{ex.Message}", "OK");
            }
        }
    }
}

/* 	private async void GeneraReport_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
		{
			bool conferma = await DisplayAlert("Conferma", $"Vuoi generare il report del cantiere di {selectedCantiere.Città}?", "Si", "No");
			if (conferma)
			{
				var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				var scriptFileName = "report_generator.py";
				var scriptPath = FindFile(baseDirectory, scriptFileName);
				// Controlla se il file esiste
				if (scriptPath == null)
				{
					await DisplayAlert("Errore", $"Il file dello script Python non è stato trovato: {scriptPath}", "OK");
					return;
				}

				// Percorso assoluto per l'interprete Python 
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

	} */

	private string FindFile(string directory, string fileName)
	{
		foreach (var file in Directory.GetFiles(directory, fileName, SearchOption.AllDirectories))
		{
			return file;
		}
		return null;
	}
}