using System.Collections.ObjectModel;
using SiteManager.Models;
using SiteManager.Services;

namespace SiteManager;

public partial class SchedaCantierePage : ContentPage
{
    public ObservableCollection<Operaio> OperaiList { get; set; }
    public ObservableCollection<Materiale> MaterialiList { get; set; }

	private readonly Cantiere cantiere;

	public SchedaCantierePage(Cantiere selectedCantiere)
	{
		InitializeComponent();
        OperaiList = [];
		MaterialiList = [];
        cantiere = selectedCantiere;
        LoadOperai();
		LoadMateriali();
	}

    private void LoadOperai()
    {
        List<Operaio> operai = OperaioService.OttieniOperai();
        foreach (Operaio operaio in operai)
        {
            OperaiList.Add(operaio);

            if(operaio.CantiereId.HasValue && operaio.CantiereId.Value == cantiere.IdCantiere) 
            {
                operaio.BackgroundColor = Colors.DarkSlateGray;
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
        List<Materiale> materiali = MaterialeService.OttieniMateriali();

        foreach (var materiale in materiali)
        {
            MaterialiList.Add(materiale);
        }
        MaterialiCollectionView.ItemsSource = MaterialiList;
    }

    private async void AssegnaOperaio_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma", $"Sei sicuro di voler assegnare {operaio.Nome} {operaio.Cognome} al cantiere?", "Sì", "No");
        if (conferma)
        {
            operaio.CantiereId = cantiere.IdCantiere;
            operaio.BackgroundColor = Colors.DarkSlateGray;
            OperaioService.AssegnaOperaioACantiere(operaio);
            await DisplayAlert("Successo", "Operaio assegnato con successo.", "OK");
            OperaiCollectionView.ItemsSource = null; 
            OperaiCollectionView.ItemsSource = OperaiList; 
        }
    }

    private async void RimuoviOperaio_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Operaio operaio = (Operaio)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma", $"Sei sicuro di voler rimuovere {operaio.Nome} {operaio.Cognome} dal cantiere?", "Sì", "No");
        if (conferma)
        {
            operaio.CantiereId = null;
            OperaioService.AggiornaOperaio(operaio);
            await DisplayAlert("Successo", "Operaio rimosso dal cantiere con successo.", "OK");
            operaio.BackgroundColor = Colors.Transparent;
            OperaiCollectionView.ItemsSource = null;
            OperaiCollectionView.ItemsSource = OperaiList;
        }
    }

    private async void AssegnaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.CommandParameter;

        var quantitaUtilizzata = await DisplayPromptAsync("Quantita", "Inserisci la quantita da assegnare:", "OK", "Annulla", "Quantita", -1, Keyboard.Numeric);
        
        if (int.TryParse(quantitaUtilizzata, out int quantita))
        {
            MaterialeService.AssegnaMaterialeACantiere(cantiere.IdCantiere, materiale.IdMateriale, quantita);
            await DisplayAlert("Successo", "Materiale assegnato con successo.", "OK");
            MaterialiList.Clear();
            LoadMateriali();
        }
        else
        {
            await DisplayAlert("Errore", "Inserisci una quantità valida.", "OK");
        }
    }


}