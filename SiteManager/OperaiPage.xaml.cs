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
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Operaio selectedOperaio)
        {
            NomeEntry.Text = selectedOperaio.Nome;
            CognomeEntry.Text = selectedOperaio.Cognome;
            MansioneLabel.Text = selectedOperaio.Mansione;
            DataNascitaPicker.Date = selectedOperaio.DataNascita;
            DataAssunzionePicker.Date = selectedOperaio.DataAssunzione;
            FormStackLayout.IsVisible = true;
        }
    }
   
    private void AggiungiOperaio_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaOperaioBtn.IsVisible = true;
        AggiornaOperaioBtn.IsVisible = false;
        AggiungiOperaioBtn.IsVisible = false;
        ClearForm();
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

        string nome = NomeEntry.Text; 
        string cognome = CognomeEntry.Text; 
        string mansione = MansioneLabel.Text; 
        decimal costoOrario = decimal.Parse(CostoOrarioLabel.Text.Replace(" €/h", "")); // Estrai il valore numerico
        DateTime dataNascita = DataNascitaPicker.Date; 
        DateTime dataAssunzione = DataAssunzionePicker.Date; 

        Operaio nuovoOperaio = new Operaio
        {
            Nome = nome,
            Cognome = cognome,
            Mansione = mansione,
            CostoOrario = costoOrario,
            DataNascita = dataNascita,
            DataAssunzione = dataAssunzione
        };

        bool success = OperaioService.AggiungiOperaio(nuovoOperaio);

        if (success)
        {
            await DisplayAlert("Successo", "Operaio aggiunto con successo", "OK");
            AggiungiOperaioBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            SalvaOperaioBtn.IsVisible = false;
            OperaiList.Add(nuovoOperaio);
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta dell'operaio", "OK");
        }
    }

    private void VisualizzaOperaio_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.BindingContext;
        if (operaio != null)
        {
            DisplayAlert("Dettagli Operaio", $"Nome: {operaio.Nome}\nCognome: {operaio.Cognome}\nMansione: {operaio.Mansione}\n" +
                        $"Data di Nascita: {operaio.DataNascita.ToShortDateString()}\nData di Assunzione: " +
                        $"{operaio.DataAssunzione.ToShortDateString()}", "OK");
        }
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

        operaio.Nome = NomeEntry.Text;                          //la logica di aggiornamento avviene anche se fallisce l'aggiunta
        operaio.Cognome = CognomeEntry.Text;
        operaio.Mansione = MansioneLabel.Text;
        operaio.DataNascita = DataNascitaPicker.Date;
        operaio.DataAssunzione = DataAssunzionePicker.Date;
            
        await DisplayAlert("Dettagli Operaio", $"Id: {operaio.IdOperaio} \nNome: {operaio.Nome}\nCognome: {operaio.Cognome}\n" +
                            $"Mansione: {operaio.Mansione}\nData di Nascita: {operaio.DataNascita.ToShortDateString()}\n" +
                            $"Data di Assunzione: {operaio.DataAssunzione.ToShortDateString()}", "OK");

        bool success = OperaioService.AggiornaOperaio(operaio);

        if (success)
        {
            AggiungiOperaioBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            AggiornaOperaioBtn.IsVisible = false;
            OperaiCollectionView.ItemsSource = null;
            OperaiCollectionView.ItemsSource = OperaiList;
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
            bool success = OperaioService.EliminaOperaio(operaio.IdOperaio);

            if (success)
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

    private void SelezionaMansione_Clicked(object sender, EventArgs e)
    {
        MostraMansioni();
    }

    private async void MostraMansioni()
    {
        Dictionary<string, decimal> mansioni = new Dictionary<string, decimal>
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