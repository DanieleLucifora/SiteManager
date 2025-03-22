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
        var materiali = MaterialeService.OttieniMateriali();
        foreach (var materiale in materiali)
        {
            MaterialiList.Add(materiale); 
        }        
        MaterialiCollectionView.ItemsSource = MaterialiList;
	}
   
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) //
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

	private void AggiungiMateriale_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaMaterialeBtn.IsVisible = true;
        AggiornaMaterialeBtn.IsVisible = false;
        AggiungiMaterialeBtn.IsVisible = false;
        ClearForm();
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
            await DisplayAlert("Successo", "Materiale aggiunto con successo", "OK");
            AggiungiMaterialeBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            SalvaMaterialeBtn.IsVisible = false;
            MaterialiList.Add(nuovoMateriale);
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del materiale", "OK");
        }
    }

    private async void VisualizzaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.BindingContext;
        if (materiale != null)
        {
            await DisplayAlert("Dettagli materiale", $"Nome: {materiale.Nome}\nQuantita: {materiale.Quantita.ToString()}" +
                                $"\nUnita: {materiale.Unita}\nData di Nascita: {materiale.CostoUnitario.ToString()}", "OK");
        }
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
        Materiale materiale = (Materiale)AggiornaMaterialeBtn.BindingContext;   //La logica non funziona
                                                                                //Viene aggiornato il materiale nella lista
                                                                                //Anche se il database non viene cambiato
        {
            materiale.Nome = NomeEntry.Text;
            materiale.Quantita = int.Parse(QuantitaEntry.Text);
            materiale.Unita = UnitaEntry.Text;
            materiale.CostoUnitario = double.Parse(CostoUnitarioEntry.Text);
            
            await DisplayAlert("Dettagli materiale", $"Id: {materiale.IdMateriale} \nNome: {materiale.Nome}\nQuantita: {materiale.Quantita}\n" +
                                $"Unita: {materiale.Unita}\nData di Nascita: {materiale.CostoUnitario}", "OK");

            bool success = MaterialeService.AggiornaMateriale(materiale);

            if (success)
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
    }

    private async void EliminaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il {materiale.Nome}?", "Si", "No");
        if (conferma)
        {
            bool success = MaterialeService.EliminaMateriale(materiale.IdMateriale);
            if (success)
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