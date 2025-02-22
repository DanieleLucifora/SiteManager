using MySql.Data.MySqlClient;
using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class TasksPage : ContentPage
{
	public ObservableCollection<Tasks> TasksList{ get; set; }	
   	private Cantiere _selectedCantiere;

	public TasksPage(Cantiere selectedCantiere)
	{
		InitializeComponent();
        TasksList = new ObservableCollection<Tasks>();
        _selectedCantiere = selectedCantiere;
        BindingContext = this;		
        LoadTasks();		
	}
    private void LoadTasks()
    {
        var tasks = TasksService.OttieniTasks(_selectedCantiere);
        foreach (var task in tasks)
        {
            TasksList.Add(task);
        }        
        TasksCollectionView.ItemsSource = TasksList;
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Tasks selectedTask)
        {
            DescrizioneEntry.Text = selectedTask.Descrizione;
            DataPicker.Date = selectedTask.Data;
            FormStackLayout.IsVisible = true;
        }
    }
    private void VisualizzaTask_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var task = button?.BindingContext as Tasks;
        if (task != null)
        {
            // Logica per visualizzare i dettagli del task
            DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione}\nData: {task.Data.ToShortDateString()}", "OK");
        }
    }

	private async void AggiungiTask_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaTaskBtn.IsVisible = true;
        AggiornaTaskBtn.IsVisible = false;
        ClearForm();
        await Task.CompletedTask;
	}
    private async void SalvaTask_Clicked(object sender, EventArgs e)
    {
        string descrizione = DescrizioneEntry.Text;
        DateTime data = DataPicker.Date;
        int cantiereId = _selectedCantiere.IdCantiere;

        Tasks nuovaTask = new Tasks
        {
            Descrizione = descrizione,
            Data = data,
            CantiereId = cantiereId
        };

        bool success = TasksService.AggiungiTask(nuovaTask);

        if (success)
        {
            TasksList.Add(nuovaTask);
            await DisplayAlert("Successo", "Task aggiunto con successo", "OK");
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del task", "OK");
        }
    }

    private async void ModificaTask_Clicked(object sender, EventArgs e)
	{
        if (sender is Button button && button.CommandParameter is Tasks selectedTask)
        {
            // Popola i campi del form con i dati del task selezionato
            DescrizioneEntry.Text = selectedTask.Descrizione;
            DataPicker.Date = selectedTask.Data;

            // Imposta il BindingContext del pulsante AggiornaOperaioBtn
            AggiornaTaskBtn.BindingContext = selectedTask;

            // Rendi visibile il form e il pulsante di aggiornamento
            FormStackLayout.IsVisible = true;
            AggiornaTaskBtn.IsVisible = true;
            SalvaTaskBtn.IsVisible = false; // Nascondi il pulsante di salvataggio se necessario

            await Task.CompletedTask;
        }
        else
        {
            await DisplayAlert("Errore", "Nessun task selezionato", "OK");
        }
	}

    private async void AggiornaTask_Clicked(object sender, EventArgs e)
    {
        if (AggiornaTaskBtn.BindingContext is Tasks selectedTask)
        {
            // Aggiorna i dati del cantiere con i valori del form
            selectedTask.Descrizione = DescrizioneEntry.Text;
            selectedTask.Data = DataPicker.Date;
            
            DisplayAlert("Dettagli Task", $"Descizione: {selectedTask.Descrizione} \nData: {selectedTask.Data.ToShortDateString()}", "OK");

            bool success = TasksService.AggiornaTask(selectedTask);
            if (success)
            {
                TasksCollectionView.ItemsSource = null;
                TasksCollectionView.ItemsSource = TasksList;
                await DisplayAlert("Successo", "Task aggiornato con successo", "OK");
                ClearForm();
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del task", "OK");
            }

        }
    }
    private async void EliminaTask_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Tasks selectedTask)
        {
            bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il task?", "Si", "No");
            if (conferma)
            {
                bool success = TasksService.EliminaTask(selectedTask.IdTasks);
                if (success)
                {
                    TasksList.Remove(selectedTask);
                    await DisplayAlert("Successo", "Task cancellato con successo", "OK"); 
                    ClearForm();                
                }
                else
                {
                    await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del task", "OK");
                }                  
            }             
        }
    } 

	private void ClearForm()
    {
        DescrizioneEntry.Text = string.Empty;
        DataPicker.Date = DateTime.Now;
    }	
}