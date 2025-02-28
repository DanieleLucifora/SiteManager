using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;

namespace SiteManager;

public partial class StatistichePage : ContentPage
{
	private readonly CantiereService _cantiereService;
	public ObservableCollection<Cantiere> CantieriList { get; set; }	
	public StatistichePage()
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

	//Metodo da rivedere !!!
	private async void GeneraStatistiche_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
		{
			bool conferma = await DisplayAlert("Conferma", $"Vuoi visualizzare le statistiche per il cantiere di {selectedCantiere.Citta}?", "Si", "No");
			if (conferma)
			{
				try
				{
					using (HttpClient client = new HttpClient())
					{
						var payload = new
						{
							cantiere = selectedCantiere.Citta  // Passiamo il nome della citta al server
						};

						var jsonContent = new StringContent(
							JsonConvert.SerializeObject(payload),
							Encoding.UTF8,
							"application/json"
						);

						string serverUrl = "http://localhost:5002/statistiche"; // URL API del server Flask
						HttpResponseMessage response = await client.PostAsync(serverUrl, jsonContent);
						string result = await response.Content.ReadAsStringAsync();

						if (response.IsSuccessStatusCode)
						{
							await DisplayAlert("Statistiche Generate", $"Le statistiche sono state create con successo:\n{result}", "OK");
						}
						else
						{
							await DisplayAlert("Errore", $"Errore nella generazione delle statistiche:\n{result}", "OK");
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
