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

        string stringa_ore = await DisplayPromptAsync("Inserimento Ore Lavorate", $"Inserisci le ore lavorate per {operaio.Nome} " + 
			$"{operaio.Cognome}:", "Salva", "Annulla", placeholder: "0.0", maxLength: 4, keyboard: Keyboard.Numeric);

		if (string.IsNullOrEmpty(stringa_ore))
		{
            await DisplayAlert("Errore", "Inserisci un valore", "OK");
            return;
        }

        if (decimal.TryParse(stringa_ore, out decimal ore))
        {
            Presenza presenzaOperaio = new()
            {
                OperaioId = operaio.IdOperaio,
                Ore = ore,
                CantiereId = cantiere.IdCantiere
            };

            bool presenza_aggiunta = PresenzaService.AggiungiPresenza(presenzaOperaio);

            if (presenza_aggiunta)
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
            await DisplayAlert("Errore", "Formato non valido", "OK");
        }
	}

	private void AggiungiSpesa_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaSpesaBtn.IsVisible = true;
		AggiungiSpesaBtn.IsVisible = false;
        ClearForm();
	}

    private void SalvaSpesa_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text) || 
            string.IsNullOrWhiteSpace(CostoEntry.Text))
        {
            DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        string descrizione = DescrizioneEntry.Text;
        if(decimal.TryParse(CostoEntry.Text, out decimal costo))
        {
            Spesa nuovaSpesa = new()
            {
                Descrizione = descrizione,
                Costo = costo,
			    CantiereId = cantiere.IdCantiere
            };

            bool spesa_aggiunta = SpesaService.AggiungiSpesa(nuovaSpesa);

            if (spesa_aggiunta)
            {
                FormStackLayout.IsVisible = false;
                SalvaSpesaBtn.IsVisible = false;
                AggiungiSpesaBtn.IsVisible = true;
                ClearForm();
                DisplayAlert("Successo", "Spesa aggiunta con successo", "OK");
            }
            else
            {
                DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta della spesa", "OK");
            }
        }
        else
        {
            DisplayAlert("Errore", "Formato non valido", "OK");
        }
    }

	private void ClearForm()
    {
        DescrizioneEntry.Text = string.Empty;
        CostoEntry.Text = string.Empty;
    }	
}