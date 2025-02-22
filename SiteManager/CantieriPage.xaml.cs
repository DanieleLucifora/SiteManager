using MySql.Data.MySqlClient;
using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class CantieriPage : ContentPage
{
    public ObservableCollection<Cantiere> CantieriList{ get; set; }
	public CantieriPage()
	{
		InitializeComponent();
        CantieriList = new ObservableCollection<Cantiere>();
        BindingContext = this;		
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
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Cantiere selectedCantiere)
        {
            CittàEntry.Text = selectedCantiere.Città;
            CommittenteEntry.Text = selectedCantiere.Committente;
            DataInizioPicker.Date = selectedCantiere.DataInizio;
            ScadenzaPicker.Date = selectedCantiere.Scadenza;
            FormStackLayout.IsVisible = true;
        }
    }
    private async void TasksCantiere_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
        {
            //var context = new DataContext();
            await Navigation.PushAsync(new TasksPage(selectedCantiere));
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un cantiere prima di procedere.", "OK");
        }        
    }
	private async void AggiungiCantiere_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaCantiereBtn.IsVisible = true;
        AggiornaCantiereBtn.IsVisible = false;
        ClearForm();
        await Task.CompletedTask;
	}

    private async void SalvaCantiere_Clicked(object sender, EventArgs e)
    {
        
        string città = CittàEntry.Text; 
        string committente = CommittenteEntry.Text;  
        DateTime dataInizio = DataInizioPicker.Date; 
        DateTime scadenza = ScadenzaPicker.Date; 

        Cantiere nuovoCantiere = new Cantiere
        {
            Città = città,
            Committente = committente,
            DataInizio = dataInizio,
            Scadenza = scadenza
        };

        bool success = CantiereService.AggiungiCantiere(nuovoCantiere);

        if (success)
        {
            CantieriList.Add(nuovoCantiere);
            await DisplayAlert("Successo", "Cantiere aggiunto con successo", "OK");
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del cantiere", "OK");
        }
    }

    private void VisualizzaCantiere_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var cantiere = button?.BindingContext as Cantiere;
        if (cantiere != null)
        {
            // Logica per visualizzare i dettagli del cantiere
            DisplayAlert("Dettagli Cantiere", $"Città: {cantiere.Città}\nCommittente: {cantiere.Committente}\nData inizio: {cantiere.DataInizio.ToShortDateString()}\nScadenza: {cantiere.Scadenza.ToShortDateString()}", "OK");
        }
    }

    private async void GestisciCantiere_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
        {
            //var context = new DataContext();
            await Navigation.PushAsync(new SchedaCantierePage(selectedCantiere));
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un cantiere prima di procedere.", "OK");
        }        
    }

    private async void ModificaCantiere_Clicked(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
        {
            // Popola i campi del form con i dati dell'operaio selezionato
            CittàEntry.Text = selectedCantiere.Città;
            CommittenteEntry.Text = selectedCantiere.Committente;
            DataInizioPicker.Date = selectedCantiere.DataInizio;
            ScadenzaPicker.Date = selectedCantiere.Scadenza;

            // Imposta il BindingContext del pulsante AggiornaOperaioBtn
            AggiornaCantiereBtn.BindingContext = selectedCantiere;

            // Rendi visibile il form e il pulsante di aggiornamento
            FormStackLayout.IsVisible = true;
            AggiornaCantiereBtn.IsVisible = true;
            SalvaCantiereBtn.IsVisible = false; // Nascondi il pulsante di salvataggio se necessario

            await Task.CompletedTask;
        }
        else
        {
            await DisplayAlert("Errore", "Nessun cantiere selezionato", "OK");
        }
	}

    private async void AggiornaCantiere_Clicked(object sender, EventArgs e)
    {
        if (AggiornaCantiereBtn.BindingContext is Cantiere selectedCantiere)
        {
            // Aggiorna i dati del cantiere con i valori del form
            selectedCantiere.Città = CittàEntry.Text;
            selectedCantiere.Committente = CommittenteEntry.Text;
            selectedCantiere.DataInizio = DataInizioPicker.Date;
            selectedCantiere.Scadenza = ScadenzaPicker.Date;
            
            DisplayAlert("Dettagli Cantiere", $"Id: {selectedCantiere.IdCantiere} \nCittà: {selectedCantiere.Città}\nCommittente: {selectedCantiere.Committente}\nData di Nascita: {selectedCantiere.DataInizio.ToShortDateString()}\nData di Assunzione: {selectedCantiere.Scadenza.ToShortDateString()}", "OK");

            bool success = CantiereService.AggiornaCantiere(selectedCantiere);
            if (success)
            {
                CantieriCollectionView.ItemsSource = null;
                CantieriCollectionView.ItemsSource = CantieriList;
                await DisplayAlert("Successo", "Cantiere aggiornato con successo", "OK");
                ClearForm();
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del cantiere", "OK");
            }

        }
    }

    private async void EliminaCantiere_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Cantiere selectedCantiere)
        {
            bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il cantiere di {selectedCantiere.Città}?", "Si", "No");
            if (conferma)
            {
                bool success = CantiereService.EliminaCantiere(selectedCantiere.IdCantiere);
                if (success)
                {
                    CantieriList.Remove(selectedCantiere);
                    await DisplayAlert("Successo", "Cantiere cancellato con successo", "OK"); 
                    ClearForm();                
                }
                else
                {
                    await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del cantiere", "OK");
                }                  
            }             
        }
    } 

	private void ClearForm()
    {
        CittàEntry.Text = string.Empty;
        CommittenteEntry.Text = string.Empty;
        DataInizioPicker.Date = DateTime.Now;
        ScadenzaPicker.Date = DateTime.Now;
    }	
}