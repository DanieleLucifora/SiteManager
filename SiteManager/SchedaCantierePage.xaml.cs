using System.Collections.ObjectModel;
using SiteManager.Models;
using SiteManager.Services;

namespace SiteManager;

public partial class SchedaCantierePage : ContentPage
{
    public ObservableCollection<Operaio> OperaiList { get; set; }
    public ObservableCollection<Materiale> MaterialiList { get; set; }
	private Cantiere _selectedCantiere;
	public SchedaCantierePage(Cantiere selectedCantiere)
	{
		InitializeComponent();
		OperaiList = new ObservableCollection<Operaio>();
		MaterialiList = new ObservableCollection<Materiale>();
        _selectedCantiere = selectedCantiere;
        BindingContext = this;		
        LoadOperai(_selectedCantiere.IdCantiere);
		LoadMateriali();
	}

    private void LoadOperai(int idCantiere)
    {
        var operai = OperaioService.OttieniOperai();
        foreach (var operaio in operai)
        {
            OperaiList.Add(operaio);
            if(operaio.CantiereId.HasValue && operaio.CantiereId.Value == idCantiere) 
            {
                operaio.BackgroundColor = Colors.Honeydew;   
            }
            else
            {
                operaio.BackgroundColor = Colors.Transparent;
            }
        }
        OperaiCollectionView.ItemsSource = OperaiList;
    }

    private void LoadMateriali()
    {
        var materiali = MaterialeService.OttieniMateriali();
        MaterialiList.Clear();
        foreach (var materiale in materiali)
        {
            MaterialiList.Add(materiale);
        }
        MaterialiCollectionView.ItemsSource = MaterialiList;
    }

    private async void AssegnaOperaio_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Operaio selectedOperaio)
        {
            bool conferma = await DisplayAlert("Conferma", $"Sei sicuro di voler assegnare {selectedOperaio.Nome} {selectedOperaio.Cognome} al cantiere?", "Sì", "No");
            if (conferma)
            {
                selectedOperaio.CantiereId = _selectedCantiere.IdCantiere;
                selectedOperaio.BackgroundColor = Colors.Honeydew;
                OperaioService.AssegnaOperaioACantiere(selectedOperaio);
                await DisplayAlert("Successo", "Operaio assegnato con successo.", "OK");
                OperaiCollectionView.ItemsSource = null; 
                OperaiCollectionView.ItemsSource = OperaiList; 
            }
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un operaio valido.", "OK");
        }
    }

    private async void RimuoviOperaio_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Operaio selectedOperaio)
        {
            bool conferma = await DisplayAlert("Conferma", $"Sei sicuro di voler rimuovere {selectedOperaio.Nome} {selectedOperaio.Cognome} dal cantiere?", "Sì", "No");
            if (conferma)
            {
                selectedOperaio.CantiereId = null;
                OperaioService.AggiornaOperaio(selectedOperaio);
                await DisplayAlert("Successo", "Operaio rimosso con successo.", "OK");
                selectedOperaio.BackgroundColor = Colors.Transparent;
                OperaiCollectionView.ItemsSource = null;
                OperaiCollectionView.ItemsSource = OperaiList;
            }
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un operaio valido.", "OK");
        }
    }

    private async void AssegnaMateriale_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Materiale selectedMateriale)
        {
            var quantitaUtilizzata = await DisplayPromptAsync("Quantita", "Inserisci la quantita da assegnare:", "OK", "Annulla", "Quantita", -1, Keyboard.Numeric);
            if (int.TryParse(quantitaUtilizzata, out int quantita))
            {
                MaterialeService.AssegnaMaterialeACantiere(_selectedCantiere.IdCantiere, selectedMateriale.IdMateriale, quantita);
                await DisplayAlert("Successo", "Materiale assegnato con successo.", "OK");
                LoadMateriali(); 
            }
            else
            {
                await DisplayAlert("Errore", "Inserisci una quantita valida.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un materiale valido.", "OK");
        }
    }

    private async void RimuoviMateriale_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Materiale selectedMateriale)
        {
            var quantitaUtilizzata = await DisplayPromptAsync("Quantita", "Inserisci la quantita da rimuovere:", "OK", "Annulla", "Quantita", -1, Keyboard.Numeric);
            if (int.TryParse(quantitaUtilizzata, out int quantita))
            {
                MaterialeService.RimuoviMaterialeDaCantiere(_selectedCantiere.IdCantiere, selectedMateriale.IdMateriale, quantita);
                await DisplayAlert("Successo", "Materiale rimosso con successo.", "OK");
                LoadMateriali();
            }
            else
            {
                await DisplayAlert("Errore", "Inserisci una quantita valida.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Errore", "Seleziona un materiale valido.", "OK");
        }
    }
}