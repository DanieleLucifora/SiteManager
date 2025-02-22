using System;
using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public class OperaioService
{
  
    public static IEnumerable<Operaio> OttieniOperai()
    {
        var operai = new List<Operaio>();
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            MySqlCommand command = new("SELECT * FROM operai", connessione); // Comando = query + db
            MySqlDataReader reader = command.ExecuteReader();
            {
                while (reader.Read())
                {
                    operai.Add(new Operaio
                    {
                        IdOperaio = reader.GetInt32("IdOperaio"),
                        Nome = reader.GetString("Nome"),
                        Cognome = reader.GetString("Cognome"),
                        Mansione = reader.GetString("Mansione"),
                        DataNascita = reader.GetDateTime("DataNascita"),
                        DataAssunzione = reader.GetDateTime("DataAssunzione")
                    });
                }
                Console.WriteLine("Lista operai caricata.");
            }
            reader.Close(); //aggiunto dopo, verificare se funziona correttamente
            connessione.Close(); //aggiunto dopo, verificare se funziona correttamente   
            return operai;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return operai;
        }
    }

    public static bool AggiungiOperaio(Operaio nuovoOperaio)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "INSERT INTO operai (Nome, Cognome, Mansione, DataNascita, DataAssunzione) VALUES (@Nome, @Cognome, @Mansione, @DataNascita, @DataAssunzione)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", nuovoOperaio.Nome);
            command.Parameters.AddWithValue("@Cognome", nuovoOperaio.Cognome);
            command.Parameters.AddWithValue("@Mansione", nuovoOperaio.Mansione);
            command.Parameters.AddWithValue("@DataNascita", nuovoOperaio.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", nuovoOperaio.DataAssunzione);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                nuovoOperaio.IdOperaio = (int)command.LastInsertedId; //recupera l'id generato automaticamente dal db per utilizzarlo eventualmente in modifica ed elimina
                Console.WriteLine("Operaio aggiunto con successo.");
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

    public bool AggiornaOperaio(Operaio operaioAggiornato)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "UPDATE operai SET Nome = @Nome, Cognome = @Cognome, Mansione = @Mansione, DataNascita = @DataNascita, DataAssunzione = @DataAssunzione WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", operaioAggiornato.Nome);
            command.Parameters.AddWithValue("@Cognome", operaioAggiornato.Cognome);
            command.Parameters.AddWithValue("@Mansione", operaioAggiornato.Mansione);
            command.Parameters.AddWithValue("@DataNascita", operaioAggiornato.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", operaioAggiornato.DataAssunzione);
            command.Parameters.AddWithValue("@IdOperaio", operaioAggiornato.IdOperaio);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Operaio aggiornato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool EliminaOperaio(int IdOperaio)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "DELETE FROM operai WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdOperaio", IdOperaio);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Operaio eliminato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }    

    public bool AssegnaOperaioACantiere(Operaio operaioAggiornato)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "UPDATE operai SET Nome = @Nome, Cognome = @Cognome, Mansione = @Mansione, DataNascita = @DataNascita, DataAssunzione = @DataAssunzione, CantiereId = @CantiereId WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", operaioAggiornato.Nome);
            command.Parameters.AddWithValue("@Cognome", operaioAggiornato.Cognome);
            command.Parameters.AddWithValue("@Mansione", operaioAggiornato.Mansione);
            command.Parameters.AddWithValue("@DataNascita", operaioAggiornato.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", operaioAggiornato.DataAssunzione);
            command.Parameters.AddWithValue("@CantiereId", operaioAggiornato.CantiereId);
            command.Parameters.AddWithValue("@IdOperaio", operaioAggiornato.IdOperaio);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Operaio aggiornato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool RimuoviOperaioDaCantiere(Operaio operaioAggiornato)
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        try
        {
            MySqlConnection connessione = new(stringaConnessione);
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            
            string query = "UPDATE operai SET CantiereId = NULL WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdOperaio", operaioAggiornato.IdOperaio);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                Console.WriteLine("Operaio rimosso dal cantiere con successo.");
            }
            else
            {
                Console.WriteLine("Errore: nessuna riga aggiornata nella tabella operai.");
            }

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