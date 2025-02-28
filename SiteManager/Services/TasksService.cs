using System;
using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public class TasksService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    
    public static IEnumerable<Tasks> OttieniTasks(Cantiere cantiere)
    {
        var taskList = new List<Tasks>();
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            MySqlCommand command = new("SELECT * FROM tasks WHERE CantiereId = @CantiereId", connessione); // Comando = query + db
            command.Parameters.AddWithValue("@CantiereId", cantiere.IdCantiere);
            MySqlDataReader reader = command.ExecuteReader();
            {
                while (reader.Read())
                {
                    taskList.Add(new Tasks
                    {
                        Descrizione = reader.GetString("Descrizione")
                    });
                }
                Console.WriteLine("Lista task caricata.");
            }

            return taskList;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return taskList;
        }
    }    

    public static bool AggiungiTask(Tasks task)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "INSERT INTO tasks (Descrizione, Data, CantiereId) VALUES (@Descrizione, @Data, @CantiereId)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Descrizione", task.Descrizione);
            command.Parameters.AddWithValue("@Data", task.Data);
            command.Parameters.AddWithValue("@CantiereId", task.CantiereId);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Task aggiunto con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool AggiornaTask(Tasks task)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "UPDATE tasks SET Descrizione = @Descrizione, Data = @Data WHERE IdTasks = @IdTasks";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Descrizione", task.Descrizione);
            command.Parameters.AddWithValue("@Data", task.Data);
            command.Parameters.AddWithValue("@IdTasks", task.IdTasks);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Task aggiornato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool EliminaTask(int IdTasks)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "DELETE FROM tasks WHERE IdTasks = @IdTasks";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdTasks", IdTasks);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Task eliminato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

}