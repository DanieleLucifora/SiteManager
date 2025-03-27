using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public static class OperaioService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    

    public static List<Operaio> OttieniOperai()
    {
        List<Operaio> operai = [];
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            MySqlCommand command = new("SELECT * FROM operai", connessione);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Operaio operaio = new()
                {
                    IdOperaio = reader.GetInt32("IdOperaio"),
                    Nome = reader.GetString("Nome"),
                    Cognome = reader.GetString("Cognome"),
                    Mansione = reader.GetString("Mansione"),
                    CostoOrario = reader.GetDecimal("CostoOrario"),
                    DataNascita = reader.GetDateTime("DataNascita"),
                    DataAssunzione = reader.GetDateTime("DataAssunzione"),
                    CantiereId = reader.IsDBNull(reader.GetOrdinal("CantiereId")) ? (int?)null : reader.GetInt32("CantiereId")
                };
                operai.Add(operaio);
            }
            reader.Close();
            connessione.Close();
            return operai;
        }
        catch (Exception)
        {
            return operai;
        }
    }

    public static List<Operaio> OttieniOperaiCantiere(int IdCantiere)
    {
        List<Operaio> operaiCantiere = [];
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            MySqlCommand command = new("SELECT * FROM operai WHERE CantiereId = @IdCantiere", connessione);
            command.Parameters.AddWithValue("@IdCantiere", IdCantiere);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                Operaio operaio = new()
                {
                    IdOperaio = reader.GetInt32("IdOperaio"),
                    Nome = reader.GetString("Nome"),
                    Cognome = reader.GetString("Cognome"),
                    Mansione = reader.GetString("Mansione"),
                    CostoOrario = reader.GetDecimal("CostoOrario"),
                    DataNascita = reader.GetDateTime("DataNascita"),
                    DataAssunzione = reader.GetDateTime("DataAssunzione")
                };
                operaiCantiere.Add(operaio);
            }
            reader.Close();
            connessione.Close();
            return operaiCantiere;
        }
        catch (Exception)
        {
            return operaiCantiere;
        }
    }

    public static bool AggiungiOperaio(Operaio operaio)
    {
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO operai (Nome, Cognome, Mansione, CostoOrario, DataNascita, DataAssunzione) VALUES (@Nome, @Cognome, @Mansione, @CostoOrario, @DataNascita, @DataAssunzione)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", operaio.Nome);
            command.Parameters.AddWithValue("@Cognome", operaio.Cognome);
            command.Parameters.AddWithValue("@Mansione", operaio.Mansione);
            command.Parameters.AddWithValue("@CostoOrario", operaio.CostoOrario);
            command.Parameters.AddWithValue("@DataNascita", operaio.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", operaio.DataAssunzione);

            int result = command.ExecuteNonQuery();
            if (result > 0) 
            {
                operaio.IdOperaio = (int)command.LastInsertedId;
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

    public static bool AggiornaOperaio(Operaio operaio)   
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE operai SET Nome = @Nome, Cognome = @Cognome, Mansione = @Mansione, CostoOrario = @CostoOrario, DataNascita = @DataNascita, DataAssunzione = @DataAssunzione, CantiereId = @CantiereId WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", operaio.Nome);
            command.Parameters.AddWithValue("@Cognome", operaio.Cognome);
            command.Parameters.AddWithValue("@Mansione", operaio.Mansione);
            command.Parameters.AddWithValue("@CostoOrario", operaio.CostoOrario);
            command.Parameters.AddWithValue("@DataNascita", operaio.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", operaio.DataAssunzione);
            command.Parameters.AddWithValue("@CantiereId", operaio.CantiereId ?? Convert.DBNull);
            command.Parameters.AddWithValue("@IdOperaio", operaio.IdOperaio);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool EliminaOperaio(int IdOperaio)
    {
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            string query = "DELETE FROM operai WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdOperaio", IdOperaio);
            int result = command.ExecuteNonQuery();

            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool AssegnaOperaioACantiere(Operaio operaio)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE operai SET Nome = @Nome, Cognome = @Cognome, Mansione = @Mansione, CostoOrario = @CostoOrario, DataNascita = @DataNascita, DataAssunzione = @DataAssunzione, CantiereId = @CantiereId WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", operaio.Nome);
            command.Parameters.AddWithValue("@Cognome", operaio.Cognome);
            command.Parameters.AddWithValue("@Mansione", operaio.Mansione);
            command.Parameters.AddWithValue("@CostoOrario", operaio.CostoOrario);
            command.Parameters.AddWithValue("@DataNascita", operaio.DataNascita);
            command.Parameters.AddWithValue("@DataAssunzione", operaio.DataAssunzione);
            command.Parameters.AddWithValue("@CantiereId", operaio.CantiereId);
            command.Parameters.AddWithValue("@IdOperaio", operaio.IdOperaio);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool RimuoviOperaioDaCantiere(Operaio operaio)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE operai SET CantiereId = NULL WHERE IdOperaio = @IdOperaio";
            MySqlCommand command = new(query, connessione);
            command.Parameters.AddWithValue("@IdOperaio", operaio.IdOperaio);

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