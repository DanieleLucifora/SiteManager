using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public static class CantiereService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    
    
    public static List<Cantiere> OttieniCantieri()
    {
        List<Cantiere> cantieri = [];
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            MySqlCommand command = new ("SELECT * FROM cantieri", connessione); //Comando = query + db
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                cantieri.Add(new Cantiere
                {
                    IdCantiere = reader.GetInt32("IdCantiere"),
                    Citta = reader.GetString("Citta"),
                    Committente = reader.GetString("Committente"),
                    DataInizio = reader.GetDateTime("DataInizio"),
                    Scadenza = reader.GetDateTime("Scadenza")
                });
            }
            reader.Close();
            connessione.Close();
            return cantieri;            
        }
        catch (Exception)
        {
            return cantieri;
        }

    }

    public static bool AggiungiCantiere(Cantiere nuovoCantiere)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO cantieri (Citta, Committente, DataInizio, Scadenza) VALUES (@Citta, @Committente, @DataInizio, @Scadenza)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Citta", nuovoCantiere.Citta);
            command.Parameters.AddWithValue("@Committente", nuovoCantiere.Committente);
            command.Parameters.AddWithValue("@DataInizio", nuovoCantiere.DataInizio);
            command.Parameters.AddWithValue("@Scadenza", nuovoCantiere.Scadenza);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                nuovoCantiere.IdCantiere = (int)command.LastInsertedId; //recupera l'id generato automaticamente dal db per utilizzarlo eventualmente in modifica ed elimina
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

    public static bool AggiornaCantiere(Cantiere cantiereAggiornato)
    {

        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE cantieri SET Citta = @Citta, Committente = @Committente, DataInizio = @DataInizio, Scadenza = @Scadenza WHERE IdCantiere = @IdCantiere";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Citta", cantiereAggiornato.Citta);
            command.Parameters.AddWithValue("@Committente", cantiereAggiornato.Committente);
            command.Parameters.AddWithValue("@DataInizio", cantiereAggiornato.DataInizio);
            command.Parameters.AddWithValue("@Scadenza", cantiereAggiornato.Scadenza);
            command.Parameters.AddWithValue("@IdCantiere", cantiereAggiornato.IdCantiere);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    
    public static bool EliminaCantiere(int IdCantiere)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "DELETE FROM cantieri WHERE IdCantiere = @IdCantiere";
            MySqlCommand command = new(query, connessione);
            command.Parameters.AddWithValue("@IdCantiere", IdCantiere);

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