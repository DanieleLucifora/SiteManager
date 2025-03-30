using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class OperaiPage : ContentPage
{
    public ObservableCollection<Operaio> OperaiList{ get; set; }

    public OperaiPage()
	{
		InitializeComponent();
        OperaiList = [];	
        LoadOperai();
	}

    private void LoadOperai()
    {
        List<Operaio> operai = OperaioService.OttieniOperai();

        foreach (Operaio operaio in operai)
        {
            OperaiList.Add(operaio); 
        }
        OperaiCollectionView.ItemsSource = OperaiList;
    }
   
    private void AggiungiOperaio_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaOperaioBtn.IsVisible = true;
        AggiungiOperaioBtn.IsVisible = false;
	}

    private async void SalvaOperaio_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||
            string.IsNullOrWhiteSpace(CognomeEntry.Text) ||
            string.IsNullOrWhiteSpace(MansioneLabel.Text) ||
            string.IsNullOrWhiteSpace(CostoOrarioLabel.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        try
        {
            Operaio nuovoOperaio = new()
            {
                Nome = NomeEntry.Text,
                Cognome = CognomeEntry.Text,
                Mansione = MansioneLabel.Text,
                CostoOrario = decimal.Parse(CostoOrarioLabel.Text.Replace(" €/h", "")), //non serve il try
                DataNascita = DataNascitaPicker.Date,
                DataAssunzione = DataAssunzionePicker.Date
            };

            bool operaioAggiunto = OperaioService.AggiungiOperaio(nuovoOperaio);

            if (operaioAggiunto)
            {
                AggiungiOperaioBtn.IsVisible = true;
                FormStackLayout.IsVisible = false;
                SalvaOperaioBtn.IsVisible = false;
                OperaiList.Add(nuovoOperaio);
                ClearForm();
                await DisplayAlert("Successo", "Operaio aggiunto con successo", "OK");
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta dell'operaio", "OK");
            }
        }
        catch
        {
            await DisplayAlert("Errore", "Problema durante la formattazione", "OK");
        }
    }

    private async void VisualizzaOperaio_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.BindingContext;
        await DisplayAlert("Dettagli Operaio", $"Nome: {operaio.Nome}\nCognome: {operaio.Cognome}\nMansione: {operaio.Mansione}\n" +
                          $"Data di Nascita: {operaio.DataNascita.ToShortDateString()}\nData di Assunzione: " +
                          $"{operaio.DataAssunzione.ToShortDateString()}", "OK");
    }

    private void ModificaOperaio_Clicked(object sender, EventArgs e)
	{
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.CommandParameter;
        
        NomeEntry.Text = operaio.Nome;
        CognomeEntry.Text = operaio.Cognome;
        MansioneLabel.Text = operaio.Mansione;
        DataNascitaPicker.Date = operaio.DataNascita;
        DataAssunzionePicker.Date = operaio.DataAssunzione;

        AggiornaOperaioBtn.BindingContext = operaio;

        FormStackLayout.IsVisible = true;
        AggiornaOperaioBtn.IsVisible = true;
        SalvaOperaioBtn.IsVisible = false;
        AggiungiOperaioBtn.IsVisible = false;
	}

    private async void AggiornaOperaio_Clicked(object sender, EventArgs e)
    {
        Operaio operaio = (Operaio)AggiornaOperaioBtn.BindingContext;

        operaio.Nome = NomeEntry.Text;
        operaio.Cognome = CognomeEntry.Text;
        operaio.Mansione = MansioneLabel.Text;
        operaio.DataNascita = DataNascitaPicker.Date;
        operaio.DataAssunzione = DataAssunzionePicker.Date;
            
        await DisplayAlert("Dettagli Operaio", $"Nome: {operaio.Nome}\nCognome: {operaio.Cognome}\n" +
                            $"Mansione: {operaio.Mansione}\nData di Nascita: {operaio.DataNascita.ToShortDateString()}\n" +
                            $"Data di Assunzione: {operaio.DataAssunzione.ToShortDateString()}", "OK");

        bool operaioAggiornato = OperaioService.AggiornaOperaio(operaio);

        if (operaioAggiornato)
        {
            AggiungiOperaioBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            AggiornaOperaioBtn.IsVisible = false;

            OperaiList.Clear();
            LoadOperai();
            await DisplayAlert("Successo", "Operaio aggiornato con successo", "OK");
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento dell'operaio", "OK");
        }
    }

    private async void EliminaOperaio_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare l'operaio {operaio.Nome} {operaio.Cognome}?", "Si", "No");
        
        if (conferma)
        {
            bool operaioEliminato = OperaioService.EliminaOperaio(operaio.IdOperaio);

            if (operaioEliminato)
            {
                OperaiList.Remove(operaio);
                await DisplayAlert("Successo", "Operaio cancellato con successo", "OK"); 
                ClearForm();                
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione dell'operaio", "OK");
            }                  
        }             
    }

    private async void SelezionaMansione_Clicked(object sender, EventArgs e)    //
    {
        Dictionary<string, decimal> mansioni = new()
        {
            {"Muratore", 12.00m},
            {"Elettricista", 15.00m},
            {"Idraulico", 15.00m},
            {"Carpentiere", 13.50m},
            {"Geometra", 16.50m},
            {"Architetto", 25.50m},
            {"Ingegnere", 32.00m}
        };

        string[] mansioniArray = mansioni.Keys.ToArray();
        string mansione = await DisplayActionSheet("Seleziona mansione", "Annulla", null, mansioniArray);

        if (!string.IsNullOrEmpty(mansione) && mansione != "Annulla")
        {
            decimal costoOrario = mansioni[mansione];
            MansioneLabel.Text = mansione;
            CostoOrarioLabel.Text = costoOrario.ToString("0.00") + " €/h";
        }
    }

	private void ClearForm()
    {
        NomeEntry.Text = string.Empty;
        CognomeEntry.Text = string.Empty;
        MansioneLabel.Text = string.Empty;
        CostoOrarioLabel.Text = string.Empty;
        DataNascitaPicker.Date = DateTime.Now;
        DataAssunzionePicker.Date = DateTime.Now;
    }	
}