using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class ChiusuraGiornataPage : ContentPage
{
	public ObservableCollection<Operaio> OperaiList { get; set; }
	private readonly Cantiere cantiere;

	public ChiusuraGiornataPage(Cantiere selectedCantiere)
	{
		InitializeComponent();
		OperaiList = [];
        cantiere = selectedCantiere;
		LoadOperai();	
	}

    private void LoadOperai()
    {
        List<Operaio> operai = OperaioService.OttieniOperaiCantiere(cantiere.IdCantiere);
        foreach (Operaio operaio in operai)
        {
			OperaiList.Add(operaio);
        }        
        OperaiCollectionView.ItemsSource = OperaiList;
    }	

	private async void OreLavorate_Clicked(object sender, EventArgs e)
	{
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.BindingContext;
        string result = await DisplayPromptAsync("Inserimento Ore Lavorate", $"Inserisci le ore lavorate per {operaio.Nome} " + 
			$"{operaio.Cognome}:", "Salva", "Annulla", placeholder: "0.0", maxLength: 5, keyboard: Keyboard.Numeric);

		if (string.IsNullOrEmpty(result))
		{
            await DisplayAlert("Errore", "Inserisci un valore", "OK");
        }
		else
		{
            if (decimal.TryParse(result, out decimal ore))  //Decimal è un value type
            {
                Presenza presenzaOperaio = new Presenza     //object initializer
                {
                    OperaioId = operaio.IdOperaio,
                    Ore = ore,
                    CantiereId = cantiere.IdCantiere
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
		}
	}

	private void AggiungiSpesa_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaSpesaBtn.IsVisible = true;
		AggiungiSpesaBtn.IsVisible = false;
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
        decimal costo = decimal.Parse(CostoEntry.Text.Replace(" €", "")); // Se viene inserito € estrai il valore numerico

        Spesa nuovaSpesa = new Spesa
        {
            Descrizione = descrizione,
            Costo = costo,
			CantiereId = cantiere.IdCantiere
        };

        bool success = SpesaService.AggiungiSpesa(nuovaSpesa);

        if (success)
        {
            await DisplayAlert("Successo", "Spesa aggiunta con successo", "OK");
            FormStackLayout.IsVisible = false;
            SalvaSpesaBtn.IsVisible = false;
            AggiungiSpesaBtn.IsVisible = true;
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