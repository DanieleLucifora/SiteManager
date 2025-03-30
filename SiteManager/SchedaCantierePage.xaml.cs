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
                operaio.BackgroundColor = Colors.DarkSlateGrey;
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

        foreach (Materiale materiale in materiali)
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
            if (operaio.CantiereId == null)
            {
                operaio.CantiereId = cantiere.IdCantiere;
                operaio.BackgroundColor = Colors.DarkSlateGrey;
                OperaioService.AssegnaOperaioACantiere(operaio);

                OperaiCollectionView.ItemsSource = null;
                OperaiCollectionView.ItemsSource = OperaiList;
                await DisplayAlert("Successo", "Operaio assegnato con successo.", "OK");
            }
            else
            {
                await DisplayAlert("Attenzione", $"Operaio assegnato ad un altro cantiere. Rimuovere l'operaio dal cantiere assegnato.", "OK");
                return;
            }
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
            operaio.BackgroundColor = Colors.Transparent;

            OperaiCollectionView.ItemsSource = null;
            OperaiCollectionView.ItemsSource = OperaiList;
            await DisplayAlert("Successo", "Operaio rimosso dal cantiere con successo.", "OK");

        }
    }

    private async void AssegnaMateriale_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Materiale materiale = (Materiale)button.CommandParameter;

        string stringaQuantità = await DisplayPromptAsync("Quantità", "Inserisci la quantità da assegnare:", "OK", "Annulla", "Quantità");
        
        try
        {
            MaterialeService.AssegnaMaterialeACantiere(cantiere.IdCantiere, materiale.IdMateriale, int.Parse(stringaQuantità));
            MaterialiList.Clear();
            LoadMateriali();
            await DisplayAlert("Successo", "Materiale assegnato con successo.", "OK");
        }
        catch
        {
            await DisplayAlert("Errore", "Inserisci una quantità valida.", "OK");
        }
    }
}