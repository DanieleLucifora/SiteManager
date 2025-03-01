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

	private async void GeneraStatistiche_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
		{
			bool conferma = await DisplayAlert("Conferma", $"Vuoi visualizzare le statistiche per il cantiere di {selectedCantiere.Citta}?", "Si", "No");
			if (conferma)
			{
				var materiali = MaterialeCantiereService.OttieniMaterialeCantiere(selectedCantiere.IdCantiere);
				
				if (!materiali.Any())
				{
					StatisticheResultLabel.Text = "Nessun materiale trovato per questo cantiere.";
					return;
				}

				Console.WriteLine("Materiali trovati:");
				foreach (var materiale in materiali)
				{
					Console.WriteLine($"ID: {materiale.IdMateriale}, Nome: {materiale.Materiale.Nome}, Quantità: {materiale.QuantitaUtilizzata}");
				}

				var jsonPayload = JsonConvert.SerializeObject(new { materiali });
				/* esempio di json inviato nella richiesta HTTP
				{
				"IdMaterialeCantiere": 1,
				"IdCantiere": 1,
				"IdMateriale": 1,
				"QuantitaUtilizzata": 50,
				"Materiale": {
					"IdMateriale": 1,
					"Nome": "Cemento",
					"Quantita": 100,
					"Unita": "kg",
					"CostoUnitario": 5.0
				}*/
				using (HttpClient client = new HttpClient())
				{
					var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

					string serverUrl = "http://localhost:5002/calcolaCostoMateriali";
					HttpResponseMessage response = await client.PostAsync(serverUrl, content);
					string result = await response.Content.ReadAsStringAsync();

					StatisticheResultLabel.Text = response.IsSuccessStatusCode 
						? $"Costo totale materiali: {result} €" 
						: $"Errore: {result}";
				}
			}
		}
	}

}
