using MySql.Data.MySqlClient;
using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace SiteManager;

public partial class OperaiPage : ContentPage
{
    private readonly OperaioService _operaioService;
    public ObservableCollection<Operaio> OperaiList{ get; set; }

    // Importa la funzione C++ dalla libreria
    [DllImport("libsortlibrary.dylib", EntryPoint = "SortStrings")]
    public static extern void SortStrings(IntPtr[] strings, int length);

    public OperaiPage()
	{
		InitializeComponent();
        OperaiList = new ObservableCollection<Operaio>();
        BindingContext = this;		
        LoadOperai();
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
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Operaio selectedOperaio)
        {
            NomeEntry.Text = selectedOperaio.Nome;
            CognomeEntry.Text = selectedOperaio.Cognome;
            MansioneEntry.Text = selectedOperaio.Mansione;
            DataNascitaPicker.Date = selectedOperaio.DataNascita;
            DataAssunzionePicker.Date = selectedOperaio.DataAssunzione;
            FormStackLayout.IsVisible = true;
        }
    }

    private void OrdinaOperai_Clicked(object sender, EventArgs e)
    {
        // Converti i nomi degli operai in puntatori a stringhe ANSI
        var operaiArray = OperaiList.Select(o => Marshal.StringToHGlobalAnsi(o.Nome)).ToArray();

        // Chiama la funzione C++ per ordinare i nomi
        SortStrings(operaiArray, operaiArray.Length);

        // Converti i puntatori ordinati di nuovo in stringhe
        var sortedNames = operaiArray.Select(ptr => Marshal.PtrToStringAnsi(ptr)).ToList();

        // Libera la memoria allocata per i puntatori
        foreach (var ptr in operaiArray)
        {
            Marshal.FreeHGlobal(ptr);
        }

        // Crea una nuova lista di operai ordinata in base ai nomi ordinati
        var sortedOperai = sortedNames.Select(name => OperaiList.First(o => o.Nome == name)).ToList();

        // Aggiorna la lista degli operai e la vista
        OperaiList = new ObservableCollection<Operaio>(sortedOperai);
        OperaiCollectionView.ItemsSource = OperaiList;
    }
    
    private async void AggiungiOperaio_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaOperaioBtn.IsVisible = true;
        AggiornaOperaioBtn.IsVisible = false;
        ClearForm();
        await Task.CompletedTask;
	}

    private async void SalvaOperaio_Clicked(object sender, EventArgs e)
    {
        
        string nome = NomeEntry.Text; 
        string cognome = CognomeEntry.Text; 
        string mansione = MansioneEntry.Text; 
        DateTime dataNascita = DataNascitaPicker.Date; 
        DateTime dataAssunzione = DataAssunzionePicker.Date; 

        Operaio nuovoOperaio = new Operaio
        {
            Nome = nome,
            Cognome = cognome,
            Mansione = mansione,
            DataNascita = dataNascita,
            DataAssunzione = dataAssunzione
        };

        bool success = OperaioService.AggiungiOperaio(nuovoOperaio);

        if (success)
        {
            OperaiList.Add(nuovoOperaio);
            await DisplayAlert("Successo", "Operaio aggiunto con successo", "OK");
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta dell'operaio", "OK");
        }
    }

    private void VisualizzaOperaio_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var operaio = button?.BindingContext as Operaio;
        if (operaio != null)
        {
            // Logica per visualizzare i dettagli dell'operaio
            DisplayAlert("Dettagli Operaio", $"Nome: {operaio.Nome}\nCognome: {operaio.Cognome}\nMansione: {operaio.Mansione}\nData di Nascita: {operaio.DataNascita.ToShortDateString()}\nData di Assunzione: {operaio.DataAssunzione.ToShortDateString()}", "OK");
        }
    }

    private async void ModificaOperaio_Clicked(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is Operaio selectedOperaio)
        {
            // Popola i campi del form con i dati dell'operaio selezionato
            NomeEntry.Text = selectedOperaio.Nome;
            CognomeEntry.Text = selectedOperaio.Cognome;
            MansioneEntry.Text = selectedOperaio.Mansione;
            DataNascitaPicker.Date = selectedOperaio.DataNascita;
            DataAssunzionePicker.Date = selectedOperaio.DataAssunzione;

            // Imposta il BindingContext del pulsante AggiornaOperaioBtn
            AggiornaOperaioBtn.BindingContext = selectedOperaio;

            // Rendi visibile il form e il pulsante di aggiornamento
            FormStackLayout.IsVisible = true;
            AggiornaOperaioBtn.IsVisible = true;
            SalvaOperaioBtn.IsVisible = false; // Nascondi il pulsante di salvataggio se necessario

            await Task.CompletedTask;
        }
        else
        {
            await DisplayAlert("Errore", "Nessun operaio selezionato", "OK");
        }
	}

    private async void AggiornaOperaio_Clicked(object sender, EventArgs e)
    {
        if (AggiornaOperaioBtn.BindingContext is Operaio selectedOperaio)
        {
            // Aggiorna i dati dell'operaio con i valori del form
            selectedOperaio.Nome = NomeEntry.Text;
            selectedOperaio.Cognome = CognomeEntry.Text;
            selectedOperaio.Mansione = MansioneEntry.Text;
            selectedOperaio.DataNascita = DataNascitaPicker.Date;
            selectedOperaio.DataAssunzione = DataAssunzionePicker.Date;
            
            await DisplayAlert("Dettagli Operaio", $"Id: {selectedOperaio.IdOperaio} \nNome: {selectedOperaio.Nome}\nCognome: {selectedOperaio.Cognome}\nMansione: {selectedOperaio.Mansione}\nData di Nascita: {selectedOperaio.DataNascita.ToShortDateString()}\nData di Assunzione: {selectedOperaio.DataAssunzione.ToShortDateString()}", "OK");

            bool success = _operaioService.AggiornaOperaio(selectedOperaio);
            if (success)
            {
                OperaiCollectionView.ItemsSource = null;
                OperaiCollectionView.ItemsSource = OperaiList;
                await DisplayAlert("Successo", "Operaio aggiornato con successo", "OK");
                ClearForm();
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento dell'operaio", "OK");
            }

        }
    }

    private async void EliminaOperaio_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Operaio selectedOperaio)
        {
            bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare l'operaio {selectedOperaio.Nome} {selectedOperaio.Cognome}?", "Si", "No");
            if (conferma)
            {
                bool success = OperaioService.EliminaOperaio(selectedOperaio.IdOperaio);
                if (success)
                {
                    OperaiList.Remove(selectedOperaio);
                    await DisplayAlert("Successo", "Operaio cancellato con successo", "OK"); 
                    ClearForm();                
                }
                else
                {
                    await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione dell'operaio", "OK");
                }                  
            }             
        }
    } 

	private void ClearForm()
    {
        NomeEntry.Text = string.Empty;
        CognomeEntry.Text = string.Empty;
        MansioneEntry.Text = string.Empty;
        DataNascitaPicker.Date = DateTime.Now;
        DataAssunzionePicker.Date = DateTime.Now;
    }	
}