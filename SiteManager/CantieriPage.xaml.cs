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

    private async void AggiungiCantiere_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaCantiereBtn.IsVisible = true;
        NuovoCantiereBtn.IsVisible = false;
        ClearForm();
        await Task.CompletedTask;
	}

    private async void SalvaCantiere_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CittaEntry.Text) || 
            string.IsNullOrWhiteSpace(CommittenteEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        string citta = CittaEntry.Text; 
        string committente = CommittenteEntry.Text;  
        DateTime dataInizio = DataInizioPicker.Date; 
        DateTime scadenza = ScadenzaPicker.Date; 

        Cantiere nuovoCantiere = new Cantiere
        {
            Citta = citta,
            Committente = committente,
            DataInizio = dataInizio,
            Scadenza = scadenza
        };

        bool success = CantiereService.AggiungiCantiere(nuovoCantiere);

        if (success)
        {
            await DisplayAlert("Successo", "Cantiere aggiunto con successo", "OK");
            FormStackLayout.IsVisible = false;
            SalvaCantiereBtn.IsVisible = false;
            NuovoCantiereBtn.IsVisible = true;
            CantieriList.Add(nuovoCantiere);
            ClearForm();
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
        await DisplayAlert("Dettagli Cantiere", $"Citta: {cantiere.Citta}\nCommittente: {cantiere.Committente}" +
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

        CittaEntry.Text = cantiere.Citta;   // Popolo i campi del form
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

        // Aggiorna i dati del cantiere con i valori del 
        cantiere.Citta = CittaEntry.Text;               //Se c'è un problema nel database, la lista viene aggiornata comunque
        cantiere.Committente = CommittenteEntry.Text;
        cantiere.DataInizio = DataInizioPicker.Date;
        cantiere.Scadenza = ScadenzaPicker.Date;
            
        await DisplayAlert("Dettagli Cantiere", "Citta: {cantiere.Citta}\nCommittente: {cantiere.Committente}\nData di inizio: " +
                            $"{cantiere.DataInizio.ToShortDateString()}\nData di scadenza: {cantiere.Scadenza.ToShortDateString()}", "OK");

        bool success = CantiereService.AggiornaCantiere(cantiere);

        if (success)
        {
            FormStackLayout.IsVisible = false;
            AggiornaCantiereBtn.IsVisible = false;
            NuovoCantiereBtn.IsVisible = true;                
            await DisplayAlert("Successo", "Cantiere aggiornato con successo", "OK");
            CantieriList.Clear();
            LoadCantieri();
            ClearForm();
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
            bool success = CantiereService.EliminaCantiere(cantiere.IdCantiere);
            if (success)
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