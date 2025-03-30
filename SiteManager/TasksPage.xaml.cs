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
        foreach (Tasks task in tasks)
        {
            TasksList.Add(task);
        }
        TasksCollectionView.ItemsSource = TasksList;
    }
    
    private async void VisualizzaTask_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Tasks task = (Tasks)button.BindingContext;
        await DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione}\nData: {task.Data.ToShortDateString()}\nId: {task.IdTasks}", "OK");
    }

	private void AggiungiTask_Clicked(object sender, EventArgs e)
	{
		FormStackLayout.IsVisible = true;
        SalvaTaskBtn.IsVisible = true;
        NuovoTaskBtn.IsVisible = false;
	}

    private async void SalvaTask_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescrizioneEntry.Text))
        {
            await DisplayAlert("Attenzione", "Tutti i campi devono essere compilati", "OK");
            return;
        }        

        Tasks nuovaTask = new()
        {
            Descrizione = DescrizioneEntry.Text,
            Data = DataPicker.Date,
            CantiereId = cantiere.IdCantiere
        };

        bool taskAggiunto = TasksService.AggiungiTask(nuovaTask);

        if (taskAggiunto)
        {
            NuovoTaskBtn.IsVisible = true;
            FormStackLayout.IsVisible = false;
            SalvaTaskBtn.IsVisible = false;

            TasksList.Add(nuovaTask);
            ClearForm();
            await DisplayAlert("Successo", "Task aggiunto con successo", "OK");
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
         
        await DisplayAlert("Dettagli Task", $"Descrizione: {task.Descrizione} \nData: {task.Data.ToShortDateString()}", "OK");

        bool TaskAggiornato = TasksService.AggiornaTask(task);

        if (TaskAggiornato)
        {
            FormStackLayout.IsVisible = false;
            AggiornaTaskBtn.IsVisible = false;
            NuovoTaskBtn.IsVisible = true;
            ClearForm();

            TasksList.Clear();
            LoadTasks();
            await DisplayAlert("Successo", "Task aggiornato con successo", "OK");
        }
        else
        {
            await DisplayAlert("Errore", "Si è verificato un errore durante l'aggiornamento del task", "OK");
        }
    }

    private async void EliminaTask_Clicked(object sender, EventArgs e)
    {
        Button button = (Button)sender;
        Tasks task = (Tasks)button.CommandParameter;

        bool conferma = await DisplayAlert("Conferma Eliminazione", $"Sei sicuro di voler cancellare il task?", "Si", "No");

        if (conferma)
        {
            bool taskEliminato = TasksService.EliminaTask(task.IdTasks);

            if (taskEliminato)
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