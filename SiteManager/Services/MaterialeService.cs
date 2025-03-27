using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public static class MaterialeService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    
        
    public static List<Materiale> OttieniMateriali()
    {
        List<Materiale> materiali = [];
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            MySqlCommand command = new("SELECT * FROM materiali", connessione);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                materiali.Add(new Materiale
                {
                    IdMateriale = reader.GetInt32("IdMateriale"),
                    Nome = reader.GetString("Nome"),
                    Quantita = reader.GetInt32("Quantita"),
                    Unita = reader.GetString("Unita"),
                    CostoUnitario = reader.GetDouble("CostoUnitario")
                });
            }
            reader.Close();
            connessione.Close();
            return materiali;
        }
        catch (Exception)
        {
            return materiali;
        }
    }

    public static bool AggiungiMateriale(Materiale nuovoMateriale)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO materiali (Nome, Quantita, Unita, CostoUnitario) VALUES (@Nome, @Quantita, @Unita, @CostoUnitario)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", nuovoMateriale.Nome);
            command.Parameters.AddWithValue("@Quantita", nuovoMateriale.Quantita);
            command.Parameters.AddWithValue("@Unita", nuovoMateriale.Unita);
            command.Parameters.AddWithValue("@CostoUnitario", nuovoMateriale.CostoUnitario);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                nuovoMateriale.IdMateriale = (int)command.LastInsertedId;
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

    public static bool AggiornaMateriale(Materiale materialeAggiornato)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "UPDATE materiali SET Nome = @Nome, Quantita = @Quantita, Unita = @Unita, CostoUnitario = @CostoUnitario WHERE IdMateriale = @IdMateriale";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", materialeAggiornato.Nome);
            command.Parameters.AddWithValue("@Quantita", materialeAggiornato.Quantita);
            command.Parameters.AddWithValue("@Unita", materialeAggiornato.Unita);
            command.Parameters.AddWithValue("@CostoUnitario", materialeAggiornato.CostoUnitario);
            command.Parameters.AddWithValue("@IdMateriale", materialeAggiornato.IdMateriale);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool EliminaMateriale(int IdMateriale)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = "DELETE FROM materiali WHERE IdMateriale = @IdMateriale";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdMateriale", IdMateriale);

            int result = command.ExecuteNonQuery();
            connessione.Close();
            return result > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }    

    public static void AssegnaMaterialeACantiere(int cantiereId, int materialeId, int quantitaUtilizzata)
    {

        try
        {
            var connessione = GetConnection();
            connessione.Open();

            string queryMateriale = "SELECT * FROM materiali WHERE IdMateriale = @IdMateriale";
            using (MySqlCommand commandMateriale = new(queryMateriale, connessione))
            {
                commandMateriale.Parameters.AddWithValue("@IdMateriale", materialeId);
                using (MySqlDataReader reader = commandMateriale.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        throw new InvalidOperationException("Materiale non trovato.");
                    }

                    int quantitaDisponibile = reader.GetInt32("Quantita");
                    if (quantitaDisponibile < quantitaUtilizzata)
                    {
                        throw new InvalidOperationException("Quantita insufficiente di materiale.");
                    }

                    Materiale materialeAggiornato = new Materiale
                    {
                        IdMateriale = reader.GetInt32("IdMateriale"),
                        Nome = reader.GetString("Nome"),
                        Quantita = quantitaDisponibile - quantitaUtilizzata,
                        Unita = reader.GetString("Unita"),
                        CostoUnitario = reader.GetDouble("CostoUnitario")
                    };

                    AggiornaMateriale(materialeAggiornato);
                }
            }

            string queryAssegnaMateriale = "INSERT INTO materialecantiere (IdCantiere, IdMateriale, QuantitaUtilizzata) VALUES (@IdCantiere, @IdMateriale, @QuantitaUtilizzata)";
            using (MySqlCommand commandAssegnaMateriale = new(queryAssegnaMateriale, connessione))
            {
                commandAssegnaMateriale.Parameters.AddWithValue("@IdCantiere", cantiereId);
                commandAssegnaMateriale.Parameters.AddWithValue("@IdMateriale", materialeId);
                commandAssegnaMateriale.Parameters.AddWithValue("@QuantitaUtilizzata", quantitaUtilizzata);
                int esecuzioneCommand = commandAssegnaMateriale.ExecuteNonQuery();
                if (esecuzioneCommand > 0)
                {
                    Console.WriteLine("Materiale assegnato al cantiere con successo.");
                }
                else
                {
                    Console.WriteLine("Errore: nessuna riga inserita nella tabella materialecantiere.");
                }
            }

            connessione.Close();

        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
        }
    }
}