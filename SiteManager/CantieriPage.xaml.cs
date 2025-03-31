using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class CantieriPage : ContentPage
{
    public ObservableCollection<Cantiere> CantieriList { get; set; }

	public CantieriPage()
	{
		InitializeComponent();
        CantieriList = [];
        LoadCantieri();
	}

    private void LoadCantieri()
    {
        List<Cantiere> cantieri = CantiereService.OttieniCantieri();
        foreach (Cantiere cantiere in cantieri)
        {
            CantieriList.Add(cantiere);   
        }
        CantieriCollectionView.ItemsSource = CantieriList;
    }

    private async void TasksCantiere_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;
        await Navigation.PushAsync(new TasksPage(cantiere));
    }
    
    private async void ChiusuraGiornata_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;
        await Navigation.PushAsync(new ChiusuraGiornataPage(cantiere));
    }

    private void AggiungiCantiere_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaCantiereBtn.IsVisible = true;
        NuovoCantiereBtn.IsVisible = false;
	}

    private async void SalvaCantiere_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CittaEntry.Text) || 
            string.IsNullOrWhiteSpace(CommittenteEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK"); 
            return;
        }

        Cantiere nuovoCantiere = new()
        {
            Citta = CittaEntry.Text,
            Committente = CommittenteEntry.Text,
            DataInizio = DataInizioPicker.Date,
            Scadenza = ScadenzaPicker.Date
        };

        bool cantiereAggiunto = CantiereService.AggiungiCantiere(nuovoCantiere);

        if (cantiereAggiunto)
        {
            FormStackLayout.IsVisible = false;
            SalvaCantiereBtn.IsVisible = false;
            NuovoCantiereBtn.IsVisible = true;
            CantieriList.Add(nuovoCantiere);
            ClearForm();
            await DisplayAlert("Successo", "Cantiere aggiunto con successo", "OK");
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del cantiere", "OK");
        }
    }

    private async void VisualizzaCantiere_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;
        await DisplayAlert("Dettagli Cantiere", $"Città: {cantiere.Citta}\nCommittente: {cantiere.Committente}" +
                            $"\nData inizio: {cantiere.DataInizio.ToShortDateString()}\nScadenza: " +
                            $"{cantiere.Scadenza.ToShortDateString()}", "OK");
    }

    private async void GestisciCantiere_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;
        await Navigation.PushAsync(new SchedaCantierePage(cantiere));    
    }

    private void ModificaCantiere_Clicked(object sender, EventArgs e)
	{
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;

        CittaEntry.Text = cantiere.Citta;
        CommittenteEntry.Text = cantiere.Committente;
        DataInizioPicker.Date = cantiere.DataInizio;
        ScadenzaPicker.Date = cantiere.Scadenza;

        AggiornaCantiereBtn.BindingContext = cantiere;

        FormStackLayout.IsVisible = true;
        AggiornaCantiereBtn.IsVisible = true;
        NuovoCantiereBtn.IsVisible = false;
        SalvaCantiereBtn.IsVisible = false;
	}

    private async void AggiornaCantiere_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;

        cantiere.Citta = CittaEntry.Text;
        cantiere.Committente = CommittenteEntry.Text;
        cantiere.DataInizio = DataInizioPicker.Date;
        cantiere.Scadenza = ScadenzaPicker.Date;

        await DisplayAlert("Dettagli Cantiere", $"Citta: {cantiere.Citta}\nCommittente: {cantiere.Committente}\nData di inizio: " +
                           $"{cantiere.DataInizio.ToShortDateString()}\nData di scadenza: {cantiere.Scadenza.ToShortDateString()}", "OK");

        bool cantiereAggiornato = CantiereService.AggiornaCantiere(cantiere);

        if (cantiereAggiornato)
        {
            FormStackLayout.IsVisible = false;
            AggiornaCantiereBtn.IsVisible = false;
            NuovoCantiereBtn.IsVisible = true;
            ClearForm();

            CantieriList.Clear();
            LoadCantieri();
            await DisplayAlert("Successo", "Cantiere aggiornato con successo", "OK");
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del cantiere", "OK");
        }
    }

    private async void EliminaCantiere_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Cantiere cantiere = (Cantiere)button.BindingContext;
        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il cantiere di {cantiere.Citta}?", "Si", "No");
        if (conferma)
        {
            bool cantiereEliminato = CantiereService.EliminaCantiere(cantiere.IdCantiere);
            if (cantiereEliminato)
            {
                CantieriList.Remove(cantiere);
                await DisplayAlert("Successo", "Cantiere cancellato con successo", "OK"); 
                ClearForm();                
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del cantiere", "OK");
            }              
        }
    }

	private void ClearForm()
    {
        CittaEntry.Text = string.Empty;
        CommittenteEntry.Text = string.Empty;
        DataInizioPicker.Date = DateTime.Now;
        ScadenzaPicker.Date = DateTime.Now;
    }
}