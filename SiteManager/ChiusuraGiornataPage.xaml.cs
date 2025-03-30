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

        string stringaOre = await DisplayPromptAsync("Inserimento Ore Lavorate", $"Inserisci le ore lavorate per {operaio.Nome} " + 
			$"{operaio.Cognome}:", "Salva", "Annulla", placeholder: "0.0", maxLength: 4);

		if (string.IsNullOrEmpty(stringaOre))
		{
            await DisplayAlert("Errore", "Inserisci un valore", "OK");
            return;
        }

        try
        {
            Presenza presenzaOperaio = new()
            {
                OperaioId = operaio.IdOperaio,
                Ore = decimal.Parse(stringaOre),
                CantiereId = cantiere.IdCantiere
            };

            bool presenzaAggiunta = PresenzaService.AggiungiPresenza(presenzaOperaio);

            if (presenzaAggiunta)
            {
                await DisplayAlert("Successo", $"Registrate {presenzaOperaio.Ore} ore per {operaio.Nome} {operaio.Cognome}", "OK");
            }
            else
            {
                await DisplayAlert("Errore", "Impossibile registrare le ore lavorate", "OK");
            }
        }
        catch
        {
            await DisplayAlert("Errore", "Formato non valido", "OK");
        }
	}

	private void AggiungiSpesa_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaSpesaBtn.IsVisible = true;
		AggiungiSpesaBtn.IsVisible = false;
	}

    private async void SalvaSpesa_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text) || 
            string.IsNullOrWhiteSpace(CostoEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        try
        {
            Spesa nuovaSpesa = new()
            {
                Descrizione = DescrizioneEntry.Text,
                Costo = decimal.Parse(CostoEntry.Text),
			    CantiereId = cantiere.IdCantiere
            };

            bool spesaAggiunta = SpesaService.AggiungiSpesa(nuovaSpesa);

            if (spesaAggiunta)
            {
                FormStackLayout.IsVisible = false;
                SalvaSpesaBtn.IsVisible = false;
                AggiungiSpesaBtn.IsVisible = true;
                ClearForm();
                await DisplayAlert("Successo", "Spesa aggiunta con successo", "OK");
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta della spesa", "OK");
            }
        }
        catch
        {
            await DisplayAlert("Errore", "Formato non valido", "OK");
        }
    }

	private void ClearForm()
    {
        DescrizioneEntry.Text = string.Empty;
        CostoEntry.Text = string.Empty;
    }	
}