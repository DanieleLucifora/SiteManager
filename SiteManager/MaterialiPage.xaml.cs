using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class MaterialiPage : ContentPage
{
	public ObservableCollection<Materiale> MaterialiList { get; set; }

	public MaterialiPage()
	{
		InitializeComponent();
		MaterialiList = [];	
        LoadMateriali();
	}

    private void LoadMateriali()
    {
        List<Materiale> materiali = MaterialeService.OttieniMateriali();
        foreach (Materiale materiale in materiali)
        {
            MaterialiList.Add(materiale); 
        }        
        MaterialiCollectionView.ItemsSource = MaterialiList;
	}

	private void AggiungiMateriale_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaMaterialeBtn.IsVisible = true;
        AggiungiMaterialeBtn.IsVisible = false;
	}

    private async void SalvaMateriale_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NomeEntry.Text) ||    
            string.IsNullOrWhiteSpace(QuantitaEntry.Text) ||
            string.IsNullOrWhiteSpace(UnitaEntry.Text) ||
            string.IsNullOrWhiteSpace(CostoUnitarioEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }

        try
        {
            Materiale nuovoMateriale = new()
            {
                Nome = NomeEntry.Text,
                Quantita = int.Parse(QuantitaEntry.Text),
                Unita = UnitaEntry.Text,
                CostoUnitario = double.Parse(CostoUnitarioEntry.Text)
            };

            bool materialeAggiunto = MaterialeService.AggiungiMateriale(nuovoMateriale);

            if (materialeAggiunto)
            {
                AggiungiMaterialeBtn.IsVisible = true;
                FormStackLayout.IsVisible = false;
                SalvaMaterialeBtn.IsVisible = false;
                MaterialiList.Add(nuovoMateriale);
                ClearForm();
                await DisplayAlert("Successo", "Materiale aggiunto con successo", "OK");
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del materiale", "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Errore", "Errore di Formattazione", "OK");
        }
    }

    private async void VisualizzaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.BindingContext;
        await DisplayAlert("Dettagli materiale", $"Nome: {materiale.Nome}\nQuantità: {materiale.Quantita}" +
                           $"\nUnità: {materiale.Unita}\nCosto: {materiale.CostoUnitario} €", "OK");
    }

    private void ModificaMateriale_Clicked(object sender, EventArgs e)
	{
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.CommandParameter;

        NomeEntry.Text = materiale.Nome;
        QuantitaEntry.Text = materiale.Quantita.ToString();
        UnitaEntry.Text = materiale.Unita;
        CostoUnitarioEntry.Text = materiale.CostoUnitario.ToString();

        AggiornaMaterialeBtn.BindingContext = materiale;

        FormStackLayout.IsVisible = true;
        AggiornaMaterialeBtn.IsVisible = true;
        SalvaMaterialeBtn.IsVisible = false;
        AggiungiMaterialeBtn.IsVisible = false;
	}

    private async void AggiornaMateriale_Clicked(object sender, EventArgs e)
    {
        Materiale materiale = (Materiale)AggiornaMaterialeBtn.BindingContext;

        try
        {
            materiale.Nome = NomeEntry.Text;
            materiale.Quantita = int.Parse(QuantitaEntry.Text);
            materiale.Unita = UnitaEntry.Text;
            materiale.CostoUnitario = double.Parse(CostoUnitarioEntry.Text);

            await DisplayAlert("Dettagli materiale", $"Nome: {materiale.Nome}\nQuantita: {materiale.Quantita}\n" +
                                $"Unità: {materiale.Unita}\nCosto Unitario: {materiale.CostoUnitario}", "OK");

            bool materialeAggiornato = MaterialeService.AggiornaMateriale(materiale);

            if (materialeAggiornato)
            {
                AggiungiMaterialeBtn.IsVisible = true;
                FormStackLayout.IsVisible = false;
                AggiornaMaterialeBtn.IsVisible = false;
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
        catch (Exception)
        {
            await DisplayAlert("Errore", "Errore di Formattazione", "OK");
        }
    }

    private async void EliminaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il {materiale.Nome}?", "Si", "No");
        if (conferma)
        {
            bool materialeEliminato = MaterialeService.EliminaMateriale(materiale.IdMateriale);
            if (materialeEliminato)
            {
                MaterialiList.Remove(materiale);
                await DisplayAlert("Successo", "Materiale cancellato con successo", "OK");  
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del materiale", "OK");
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