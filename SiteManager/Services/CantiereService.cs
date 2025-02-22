using System;
using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public class CantiereService
{

    public static IEnumerable<Cantiere> OttieniCantieri()
    {
        var cantieri = new List<Cantiere>();
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            MySqlCommand command = new ("SELECT * FROM cantieri", connessione); //Comando = query + db
            MySqlDataReader reader = command.ExecuteReader();
            {
                while (reader.Read())
                {
                    cantieri.Add(new Cantiere
                    {
                        IdCantiere = reader.GetInt32("IdCantiere"),
                        Città = reader.GetString("Città"),
                        Committente = reader.GetString("Committente"),
                        DataInizio = reader.GetDateTime("DataInizio"),
                        Scadenza = reader.GetDateTime("Scadenza")
                    });
                }
                Console.WriteLine("Lista cantieri caricata.");
            }
            reader.Close();
            connessione.Close();
            return cantieri;            
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return cantieri;
        }

    }

    public static bool AggiungiCantiere(Cantiere nuovoCantiere)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";

        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "INSERT INTO cantieri (Città, Committente, DataInizio, Scadenza) VALUES (@Città, @Committente, @DataInizio, @Scadenza)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Città", nuovoCantiere.Città);
            command.Parameters.AddWithValue("@Committente", nuovoCantiere.Committente);
            command.Parameters.AddWithValue("@DataInizio", nuovoCantiere.DataInizio);
            command.Parameters.AddWithValue("@Scadenza", nuovoCantiere.Scadenza);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                nuovoCantiere.IdCantiere = (int)command.LastInsertedId; //recupera l'id generato automaticamente dal db per utilizzarlo eventualmente in modifica ed elimina
                Console.WriteLine("Cantiere aggiunto con successo.");
                return true;
            }
            connessione.Close();
            return false;

        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool AggiornaCantiere(Cantiere cantiereAggiornato)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";

        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "UPDATE cantieri SET Città = @Città, Committente = @Committente, DataInizio = @DataInizio, Scadenza = @Scadenza WHERE IdCantiere = @IdCantiere";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Città", cantiereAggiornato.Città);
            command.Parameters.AddWithValue("@Committente", cantiereAggiornato.Committente);
            command.Parameters.AddWithValue("@DataInizio", cantiereAggiornato.DataInizio);
            command.Parameters.AddWithValue("@Scadenza", cantiereAggiornato.Scadenza);
            command.Parameters.AddWithValue("@IdCantiere", cantiereAggiornato.IdCantiere);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Cantiere aggiornato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }
    
    public static bool EliminaCantiere(int IdCantiere)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "DELETE FROM cantieri WHERE IdCantiere = @IdCantiere";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdCantiere", IdCantiere);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Cantiere eliminato con successo.");

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