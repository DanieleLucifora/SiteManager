using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;

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
			bool conferma = await DisplayAlert("Conferma", $"Vuoi generare il report del cantiere di {selectedCantiere.Citta}?", "Si", "No");
			if (conferma)
			{
				try
				{
					var tasks = TasksService.OttieniTasks(selectedCantiere);
                    var materiali = MaterialeCantiereService.OttieniMaterialeCantiere(selectedCantiere.IdCantiere);

					using (HttpClient client = new HttpClient())
					{
						var payload = new
						{
							cantiere = selectedCantiere.Citta,  // Passiamo il nome della citta al server
							tasks = tasks,
                            materiali = materiali
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
							await DisplayAlert("Report Generato", $"Il report è stato creato con successo!\nScaricalo dalla cartella /app del container report.", "OK");
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

}