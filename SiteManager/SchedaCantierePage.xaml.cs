using System.Collections.ObjectModel;
using SiteManager.Models;
using SiteManager.Services;

namespace SiteManager;

public partial class SchedaCantierePage : ContentPage
{
    private readonly OperaioService _operaioService;
    private readonly MaterialeService _materialeService;
    private readonly CantiereService _cantiereService;
    public ObservableCollection<Operaio> OperaiList { get; set; }
    public ObservableCollection<Materiale> MaterialiList { get; set; }
	private Cantiere _selectedCantiere;
	public SchedaCantierePage(Cantiere selectedCantiere)
	{
		InitializeComponent();
        _operaioService = new OperaioService();
        _materialeService = new MaterialeService();
        _cantiereService = new CantiereService();
		OperaiList = new ObservableCollection<Operaio>();
		MaterialiList = new ObservableCollection<Materiale>();
        _selectedCantiere = selectedCantiere;
        BindingContext = this;		
        LoadOperai();
		LoadMateriali();
	}

    private void LoadOperai()
    {
        var operai = OperaioService.OttieniOperai();
        foreach (var operaio in operai)
        {
            OperaiList.Add(operaio);
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
                _operaioService.AssegnaOperaioACantiere(selectedOperaio);
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
                _operaioService.AggiornaOperaio(selectedOperaio);
                await DisplayAlert("Successo", "Operaio rimosso con successo.", "OK");
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
                _materialeService.AssegnaMaterialeACantiere(_selectedCantiere.IdCantiere, selectedMateriale.IdMateriale, quantita);
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
                _materialeService.RimuoviMaterialeDaCantiere(_selectedCantiere.IdCantiere, selectedMateriale.IdMateriale, quantita);
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