using MySql.Data.MySqlClient;
using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class MaterialiPage : ContentPage
{
	public ObservableCollection<Materiale> MaterialiList{ get; set; }
	public MaterialiPage()
	{
		InitializeComponent();
		MaterialiList = new ObservableCollection<Materiale>();
        BindingContext = this;		
        LoadMateriali();
	}

    private void LoadMateriali()
    {
        var materiali = MaterialeService.OttieniMateriali();
        foreach (var materiale in materiali)
        {
            MaterialiList.Add(materiale);
        }        
        MaterialiCollectionView.ItemsSource = MaterialiList;
	}
   
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Materiale selectedMateriale)
        {
            NomeEntry.Text = selectedMateriale.Nome;
            QuantitaEntry.Text = selectedMateriale.Quantita.ToString();
            UnitaEntry.Text = selectedMateriale.Unita;
            CostoUnitarioEntry.Text = selectedMateriale.CostoUnitario.ToString();
            FormStackLayout.IsVisible = true;
        }
    }

	private async void AggiungiMateriale_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaMaterialeBtn.IsVisible = true;
        AggiornaMaterialeBtn.IsVisible = false;
        ClearForm();
        await Task.CompletedTask;
	}

    private async void SalvaMateriale_Clicked(object sender, EventArgs e)
    {
        
        string nome = NomeEntry.Text; 
        int Quantita = int.Parse(QuantitaEntry.Text); 
        string Unita = UnitaEntry.Text; 
        double CostoUnitario = double.Parse(CostoUnitarioEntry.Text); 

        Materiale nuovoMateriale = new Materiale
        {
            Nome = nome,
            Quantita = Quantita,
            Unita = Unita,
            CostoUnitario = CostoUnitario
        };

        bool success = MaterialeService.AggiungiMateriale(nuovoMateriale);

        if (success)
        {
            MaterialiList.Add(nuovoMateriale);
            await DisplayAlert("Successo", "Materiale aggiunto con successo", "OK");
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del materiale", "OK");
        }
    }

    private void VisualizzaMateriale_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var materiale = button?.BindingContext as Materiale;
        if (materiale != null)
        {
            // Logica per visualizzare i dettagli del materiale
            DisplayAlert("Dettagli materiale", $"Nome: {materiale.Nome}\nQuantita: {materiale.Quantita.ToString()}\nUnita: {materiale.Unita}\nData di Nascita: {materiale.CostoUnitario.ToString()}", "OK");
        }
    }

    private async void ModificaMateriale_Clicked(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is Materiale selectedMateriale)
        {
            // Popola i campi del form con i dati del materiale selezionato
            NomeEntry.Text = selectedMateriale.Nome;
            QuantitaEntry.Text = selectedMateriale.Quantita.ToString();
            UnitaEntry.Text = selectedMateriale.Unita;
            CostoUnitarioEntry.Text = selectedMateriale.CostoUnitario.ToString();

            // Imposta il BindingContext del pulsante AggiornaMaterialeBtn
            AggiornaMaterialeBtn.BindingContext = selectedMateriale;

            // Rendi visibile il form e il pulsante di aggiornamento
            FormStackLayout.IsVisible = true;
            AggiornaMaterialeBtn.IsVisible = true;
            SalvaMaterialeBtn.IsVisible = false; // Nascondi il pulsante di salvataggio se necessario

            await Task.CompletedTask;
        }
        else
        {
            await DisplayAlert("Errore", "Nessun materiale selezionato", "OK");
        }
	}

    private async void AggiornaMateriale_Clicked(object sender, EventArgs e)
    {
        if (AggiornaMaterialeBtn.BindingContext is Materiale selectedMateriale)
        {
            // Aggiorna i dati del materiale con i valori del form
            selectedMateriale.Nome = NomeEntry.Text;
            selectedMateriale.Quantita = int.Parse(QuantitaEntry.Text);
            selectedMateriale.Unita = UnitaEntry.Text;
            selectedMateriale.CostoUnitario = double.Parse(CostoUnitarioEntry.Text);
            
            DisplayAlert("Dettagli materiale", $"Id: {selectedMateriale.IdMateriale} \nNome: {selectedMateriale.Nome}\nQuantita: {selectedMateriale.Quantita}\nUnita: {selectedMateriale.Unita}\nData di Nascita: {selectedMateriale.CostoUnitario.ToString()}", "OK");

            bool success = MaterialeService.AggiornaMateriale(selectedMateriale);
            if (success)
            {
                MaterialiCollectionView.ItemsSource = null;
                MaterialiCollectionView.ItemsSource = MaterialiList;
         await DisplayAlert("Successo", "Materiale aggiornato con successo", "OK");
                ClearForm();
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del materiale", "OK");
            }

        }
    }

    private async void EliminaMateriale_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Materiale selectedMateriale)
        {
            bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il  {selectedMateriale.Nome}?", "Si", "No");
            if (conferma)
            {
                bool success = MaterialeService.EliminaMateriale(selectedMateriale.IdMateriale);
                if (success)
                {
                    MaterialiList.Remove(selectedMateriale);
                    await DisplayAlert("Successo", "Materiale cancellato con successo", "OK"); 
                    ClearForm();                
                }
                else
                {
                    await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del materiale", "OK");
                }                  
            }             
        }
    } 

	private void ClearForm()
    {
        NomeEntry.Text = string.Empty;
        QuantitaEntry.Text = string.Empty;
        UnitaEntry.Text = string.Empty;
        CostoUnitarioEntry.Text = string.Empty;
    }
}