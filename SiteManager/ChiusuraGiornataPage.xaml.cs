using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class ChiusuraGiornataPage : ContentPage
{
	public ObservableCollection<Operaio> OperaiList{ get; set; }
	private Cantiere _selectedCantiere;
	public ChiusuraGiornataPage(Cantiere selectedCantiere)
	{
		InitializeComponent();
		OperaiList = [];
        _selectedCantiere = selectedCantiere;
        BindingContext = this;	
		LoadOperai();	
	}

    private void LoadOperai()
    {
        List<Operaio> operai = OperaioService.OttieniOperaiCantiere(_selectedCantiere.IdCantiere);
        foreach (Operaio operaio in operai)
        {
            bool operaioEsistente = false;
            foreach (var iOperaio in OperaiList)
            {
                if (iOperaio.IdOperaio == operaio.IdOperaio)
                {
                    operaioEsistente = true;
                    break;
                }
            }

            if (!operaioEsistente)
            {
                OperaiList.Add(operaio);
            }            
        }        
        OperaiCollectionView.ItemsSource = OperaiList;
    }	

	private async void OreLavorate_Clicked(object sender, EventArgs e)
	{
		if (sender is Button button && button.BindingContext is Operaio operaio)
		{
			string result = await DisplayPromptAsync(
				"Inserimento Ore Lavorate", 
				$"Inserisci le ore lavorate per {operaio.Nome} {operaio.Cognome}:",
				"Salva",
				"Annulla",
				placeholder: "0.0",
				maxLength: 5,
				keyboard: Keyboard.Numeric);
			
			if (!string.IsNullOrEmpty(result))
			{
				if (decimal.TryParse(result, out decimal ore))
				{
					Presenza presenzaOperaio = new Presenza
					{
						OperaioId = operaio.IdOperaio,
						Ore = ore,
						CantiereId = _selectedCantiere.IdCantiere
					};
					
					bool success = PresenzaService.AggiungiPresenza(presenzaOperaio);
					
					if (success)
					{
						await DisplayAlert("Successo", $"Registrate {ore} ore per {operaio.Nome} {operaio.Cognome}", "OK");
					}
					else
					{
						await DisplayAlert("Errore", "Impossibile registrare le ore lavorate", "OK");
					}
				}
				else
				{
					await DisplayAlert("Errore", "Inserisci un valore numerico valido", "OK");
				}
			}
		}
		else
		{
			await DisplayAlert("Errore", "Impossibile identificare l'operaio", "OK");
		}
	}

	private async void AggiungiSpesa_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaSpesaBtn.IsVisible = true;
        ClearForm();
	}

    private async void SalvaSpesa_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text) || 
            string.IsNullOrWhiteSpace(CostoEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        string descrizione = DescrizioneEntry.Text; 
        decimal costo = decimal.Parse(CostoEntry.Text.Replace(" €", "")); // Estrai il valore numerico

        Spesa nuovaSpesa = new Spesa
        {
            Descrizione = descrizione,
            Costo = costo,
			CantiereId = _selectedCantiere.IdCantiere
        };

        bool success = SpesaService.AggiungiSpesa(nuovaSpesa);

        if (success)
        {
            await DisplayAlert("Successo", "Spesa aggiunta con successo", "OK");
            FormStackLayout.IsVisible = false;
            SalvaSpesaBtn.IsVisible = false;
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta della spesa", "OK");
        }
    }

	private void ClearForm()
    {
        DescrizioneEntry.Text = string.Empty;
        CostoEntry.Text = string.Empty;
    }	
}