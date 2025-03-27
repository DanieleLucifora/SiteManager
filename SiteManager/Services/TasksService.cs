using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public static class TasksService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }

    public static List<Tasks> OttieniTasks(Cantiere cantiere)
    {
        List<Tasks> tasks = [];
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = $"SELECT * FROM tasks WHERE CantiereId = '{cantiere.IdCantiere}'";
            MySqlCommand command = new(query, connessione); 
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tasks.Add(new Tasks
                {
                    IdTasks = reader.GetInt32("IdTask"),
                    Descrizione = reader.GetString("Descrizione"),
                    Data = reader.GetDateTime("Data")
                });
            }
            return tasks;
        }
        catch (Exception)
        {
            return tasks;
        }
    }    

    public static bool AggiungiTask(Tasks task)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO tasks (Descrizione, Data, CantiereId) VALUES (@Descrizione, @Data, @CantiereId)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Descrizione", task.Descrizione);
            command.Parameters.AddWithValue("@Data", task.Data);
            command.Parameters.AddWithValue("@CantiereId", task.CantiereId);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                task.IdTasks = (int)command.LastInsertedId;
                return true;
            }
            connessione.Close();
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool AggiornaTask(Tasks task)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE tasks SET Descrizione = @Descrizione, Data = @Data WHERE IdTask = @IdTasks";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Descrizione", task.Descrizione);
            command.Parameters.AddWithValue("@Data", task.Data);
            command.Parameters.AddWithValue("@IdTasks", task.IdTasks);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool EliminaTask(int IdTasks)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "DELETE FROM tasks WHERE IdTask = @IdTask";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdTask", IdTasks);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}