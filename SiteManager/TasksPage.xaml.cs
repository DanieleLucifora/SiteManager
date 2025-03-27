using SiteManager.Models;
using SiteManager.Services;
using System.Collections.ObjectModel;

namespace SiteManager;

public partial class TasksPage : ContentPage
{
	public ObservableCollection<Tasks> TasksList { get; set; }	
   	private readonly Cantiere cantiere;

	public TasksPage(Cantiere selectedCantiere)
	{
		InitializeComponent();
        TasksList = [];
        cantiere = selectedCantiere;	
        LoadTasks();		
	}

    private void LoadTasks()
    {
        List<Tasks> tasks = TasksService.OttieniTasks(cantiere);
        foreach (var task in tasks)
        {
            TasksList.Add(task);
        }
        TasksCollectionView.ItemsSource = TasksList;
    }
    
    private void VisualizzaTask_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Tasks task = (Tasks)button.BindingContext;
        if (task != null)
        {
            DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione}\nData: {task.Data.ToShortDateString()}\nId: {task.IdTasks}", "OK");
        }
    }

	private void AggiungiTask_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaTaskBtn.IsVisible = true;
        NuovoTaskBtn.IsVisible = false;
        ClearForm();
	}

    private void SalvaTask_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text))
        {
            DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }        
        
        string descrizione = DescrizioneEntry.Text;
        DateTime data = DataPicker.Date;

        Tasks nuovaTask = new()
        {
            Descrizione = descrizione,
            Data = data,
            CantiereId = cantiere.IdCantiere
        };

        bool task_aggiunta = TasksService.AggiungiTask(nuovaTask);

        if (task_aggiunta)
        {
            NuovoTaskBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            SalvaTaskBtn.IsVisible = false;

            TasksList.Add(nuovaTask);
            ClearForm();
            DisplayAlert("Successo", "Task aggiunto con successo", "OK");
        }
        else
        {
            DisplayAlert("Errore", "Si è verificato un errore durante l'aggiunta del task", "OK");
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

    private void AggiornaTask_Clicked(object sender, EventArgs e)
    {
        Tasks task = (Tasks)AggiornaTaskBtn.BindingContext;

        task.Descrizione = DescrizioneEntry.Text;
        task.Data = DataPicker.Date;
         
        DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione} \nData: {task.Data.ToShortDateString()}", "OK");

        bool success = TasksService.AggiornaTask(task);

        if (success)
        {
            FormStackLayout.IsVisible = false;
            AggiornaTaskBtn.IsVisible = false;
            NuovoTaskBtn.IsVisible = true;
            ClearForm();

            TasksList.Clear();
            LoadTasks();
            DisplayAlert("Successo", "Task aggiornato con successo", "OK");
        }
        else
        {
            DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del task", "OK");
        }
    }

    private async void EliminaTask_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Tasks task = (Tasks)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il task?", "Si", "No");

        if (conferma)
        {
            bool task_eliminato = TasksService.EliminaTask(task.IdTasks);

            if (task_eliminato)
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