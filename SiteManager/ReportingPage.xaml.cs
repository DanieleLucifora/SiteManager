using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;

namespace SiteManager;

public partial class ReportingPage : ContentPage
{
	public ObservableCollection<Cantiere> CantieriList { get; set; }

	public ReportingPage()
	{
		InitializeComponent();
		CantieriList = [];
		LoadCantieri();
	}

	private void LoadCantieri()
	{
		List<Cantiere> cantieri = CantiereService.OttieniCantieri();
		foreach (Cantiere cantiere in cantieri)
		{
			CantieriList.Add(cantiere);
		}
		CantieriCollectionView.ItemsSource = CantieriList;
	}

	private async void GeneraReport_Clicked(object sender, EventArgs e)
	{
		Button button = (Button)sender;
		Cantiere cantiere = (Cantiere)button.CommandParameter;

		bool conferma = await DisplayAlert("Conferma", $"Vuoi generare il report del cantiere di {cantiere.Citta}?", "Si", "No");

		if (conferma)
		{
			try
			{
				List<Tasks> tasks = TasksService.OttieniTasks(cantiere);
                List<MaterialeCantiere> materiali = MaterialeCantiereService.OttieniMaterialeCantiere(cantiere.IdCantiere);
                List<Spesa> costi = SpesaService.OttieniSpese(cantiere);

                var payload = new
                {
                    cantiere = cantiere.Citta,
                    tasks,
                    materiali,
                    costi
                };

                StringContent jsonContent = new(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json"
                );

				string urlServer = "http://localhost:5001/genera_report";

				HttpClient client = new();
                HttpResponseMessage response = await client.PostAsync(urlServer, jsonContent);
                string result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Report Generato", $"Il report è stato creato con successo!\n" +
										$"Scaricalo dalla cartella /app del container report.", "OK");
                }
                else
                {
                    await DisplayAlert("Errore", $"Errore nella generazione del report:\n{result}", "OK");
                }
            }
			catch (Exception ex)
			{
				await DisplayAlert("Errore", $"Si è verificato un errore:\n{ex.Message}", "OK");
			}
		}
	}

}