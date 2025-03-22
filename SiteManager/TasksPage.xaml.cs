using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class TasksPage : ContentPage
{
	public ObservableCollection<Tasks> TasksList{ get; set; }	
   	private Cantiere cantiere;

	public TasksPage(Cantiere selectedCantiere)
	{
		InitializeComponent();
        TasksList = [];
        cantiere = selectedCantiere;	
        LoadTasks();		
	}

    private void LoadTasks()
    {
        var tasks = TasksService.OttieniTasks(cantiere);

        foreach (var task in tasks)
        {
            TasksList.Add(task);
        }
        TasksCollectionView.ItemsSource = TasksList;
    }
    
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) //
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
        Button button = (Button)sender;
        Tasks task = (Tasks)button.BindingContext;
        if (task != null)
        {
            DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione}\nData: {task.Data.ToShortDateString()}", "OK");
        }
    }

	private void AggiungiTask_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaTaskBtn.IsVisible = true;
        NuovoTaskBtn.IsVisible = false;
        ClearForm();
	}
    private async void SalvaTask_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }        
        
        string descrizione = DescrizioneEntry.Text;
        DateTime data = DataPicker.Date;

        Tasks nuovaTask = new Tasks
        {
            Descrizione = descrizione,
            Data = data,
            CantiereId = cantiere.IdCantiere
        };

        bool success = TasksService.AggiungiTask(nuovaTask);

        if (success)
        {
            await DisplayAlert("Successo", "Task aggiunto con successo", "OK");
            NuovoTaskBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            SalvaTaskBtn.IsVisible = false;
            TasksList.Add(nuovaTask);
            ClearForm();
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del task", "OK");
        }
    }

    private void ModificaTask_Clicked(object sender, EventArgs e)
	{
        Button button = (Button)sender;
        Tasks task = (Tasks)button.CommandParameter;
        
        DescrizioneEntry.Text = task.Descrizione;
        DataPicker.Date = task.Data;

        AggiornaTaskBtn.BindingContext = task;

        FormStackLayout.IsVisible = true;
        AggiornaTaskBtn.IsVisible = true;
        NuovoTaskBtn.IsVisible = false;
        SalvaTaskBtn.IsVisible = false;
	}

    private async void AggiornaTask_Clicked(object sender, EventArgs e)
    {
        Tasks task = (Tasks)AggiornaTaskBtn.BindingContext;

        task.Descrizione = DescrizioneEntry.Text;
        task.Data = DataPicker.Date;
         
        await DisplayAlert("Dettagli Task", $"Id Task: {task.IdTasks}\nDescrizione: {task.Descrizione} \nData: {task.Data.ToShortDateString()}", "OK");

        bool success = TasksService.AggiornaTask(task);

        if (success)
        {
            await DisplayAlert("Successo", "Task aggiornato con successo", "OK");
            TasksCollectionView.ItemsSource = null;
            TasksCollectionView.ItemsSource = TasksList;
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del task", "OK");
        }

        FormStackLayout.IsVisible = false;
        AggiornaTaskBtn.IsVisible = false;
        NuovoTaskBtn.IsVisible = true;
        ClearForm();
    }

    private async void EliminaTask_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Tasks task = (Tasks)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il task?", "Si", "No");

        if (conferma)
        {
            bool success = TasksService.EliminaTask(task.IdTasks);

            if (success)
            {
                TasksList.Remove(task);
                await DisplayAlert("Successo", "Task cancellato con successo", "OK");                
            }
            else
            {
                await DisplayAlert("Errore", "Si è verificato un errore durante la cancellazione del task", "OK");
            }                  
        }
    } 

	private void ClearForm()
    {
        DescrizioneEntry.Text = string.Empty;
        DataPicker.Date = DateTime.Now;
    }	
}