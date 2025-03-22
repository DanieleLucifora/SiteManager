using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;
using SkiaSharp;
using Microcharts;
using Microcharts.Maui;

namespace SiteManager;

public partial class StatistichePage : ContentPage
{
	public ObservableCollection<Cantiere> CantieriList { get; set; }

	public StatistichePage()
	{
		InitializeComponent();
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
				try
				{
					chartView.Chart = null;

					var operai = OperaioService.OttieniOperaiCantiere(selectedCantiere.IdCantiere);
					var materiali = MaterialeCantiereService.OttieniMaterialeCantiere(selectedCantiere.IdCantiere);
					var presenze = PresenzaService.OttieniPresenze(selectedCantiere);
					var spese = SpesaService.OttieniSpese(selectedCantiere);

					using (HttpClient client = new HttpClient())
					{
						var payload = new
						{
							cantiere = selectedCantiere.Citta, 
							operai,
                            materiali,
							presenze,
							spese
						};

						var jsonContent = new StringContent(
							JsonConvert.SerializeObject(payload),
							Encoding.UTF8,
							"application/json"
						);

						string serverUrl = "http://localhost:5002/calcolaStatistiche";
						HttpResponseMessage response = await client.PostAsync(serverUrl, jsonContent);
						string resultJson = await response.Content.ReadAsStringAsync();
						
						if (response.IsSuccessStatusCode)
						{

							dynamic result = JsonConvert.DeserializeObject(resultJson);
							
							double costoMateriali = result.costoMateriali;
							double costoPersonale = result.costoPersonale;
							double speseCantiere = result.speseCantiere;
							double totale = result.totale;				

							await DisplayAlert("Statistiche Generate", $"Le statistiche sono state create con successo!", "OK");

							StatisticheResultLabel.Text = $"Totale: {totale:F2} €";			

							GeneraGraficoATorta(costoMateriali, costoPersonale, speseCantiere);		

							spaceChartView.IsVisible = true;
						}
						else
						{
							await DisplayAlert("Errore", $"Errore nella generazione delle statistiche:\n{resultJson}", "OK");
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

	private void GeneraGraficoATorta(double costoMateriali, double costoPersonale, double speseCantiere)
    {
        var entries = new[]
        {
            new ChartEntry((float)costoMateriali)
            {
                Label = "Materiali",
                ValueLabel = $"{costoMateriali:F2} €",
                Color = SKColor.Parse("#4CAF50"),
                ValueLabelColor = SKColor.Parse("#4CAF50") // Set the color of the value label
            },
            new ChartEntry((float)costoPersonale)
            {
                Label = "Personale",
                ValueLabel = $"{costoPersonale:F2} €",
                Color = SKColor.Parse("#5C6BC0"),
                ValueLabelColor = SKColor.Parse("#5C6BC0") // Set the color of the value label
            },
            new ChartEntry((float)speseCantiere)
            {
                Label = "Spese",
                ValueLabel = $"{speseCantiere:F2} €",
                Color = SKColor.Parse("#FFA726"),
                ValueLabelColor = SKColor.Parse("#FFA726") // Set the color of the value label
            }
        };

        var chart = new PieChart
        {
            Entries = entries,
            BackgroundColor = SKColors.Transparent,
        };

        chartView.Chart = chart;
    }

}
