using System;
using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services;

public class MaterialeService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    
        
    public static IEnumerable<Materiale> OttieniMateriali()
    {
        var materiali = new List<Materiale>();

        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            MySqlCommand command = new("SELECT * FROM materiali", connessione); // Comando = query + db
            MySqlDataReader reader = command.ExecuteReader();
            {
                while (reader.Read())
                {
                    materiali.Add(new Materiale
                    {
                        IdMateriale = reader.GetInt32("IdMateriale"),
                        Nome = reader.GetString("Nome"),
                        Quantità = reader.GetInt32("Quantità"),
                        Unità = reader.GetString("Unità"),
                        CostoUnitario = reader.GetDouble("CostoUnitario")
                    });
                }
                Console.WriteLine("Lista materiali caricata.");
            }
            reader.Close(); //aggiunto dopo, verificare se funziona correttamente
            connessione.Close(); //aggiunto dopo, verificare se funziona correttamente 
            return materiali;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return materiali;
        }
    }

    public static bool AggiungiMateriale(Materiale nuovoMateriale)
    {

        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "INSERT INTO materiali (Nome, Quantità, Unità, CostoUnitario) VALUES (@Nome, @Quantità, @Unità, @CostoUnitario)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", nuovoMateriale.Nome);
            command.Parameters.AddWithValue("@Quantità", nuovoMateriale.Quantità);
            command.Parameters.AddWithValue("@Unità", nuovoMateriale.Unità);
            command.Parameters.AddWithValue("@CostoUnitario", nuovoMateriale.CostoUnitario);

            int result = command.ExecuteNonQuery();
            if (result > 0)
            {
                nuovoMateriale.IdMateriale = (int)command.LastInsertedId; //recupera l'id generato automaticamente dal db per utilizzarlo eventualmente in modifica ed elimina
                Console.WriteLine("Materiale aggiunto con successo.");
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

    public static bool AggiornaMateriale(Materiale materialeAggiornato)
    {

        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "UPDATE materiali SET Nome = @Nome, Quantità = @Quantità, Unità = @Unità, CostoUnitario = @CostoUnitario WHERE IdMateriale = @IdMateriale";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Nome", materialeAggiornato.Nome);
            command.Parameters.AddWithValue("@Quantità", materialeAggiornato.Quantità);
            command.Parameters.AddWithValue("@Unità", materialeAggiornato.Unità);
            command.Parameters.AddWithValue("@CostoUnitario", materialeAggiornato.CostoUnitario);
            command.Parameters.AddWithValue("@IdMateriale", materialeAggiornato.IdMateriale);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Materiale aggiornato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }

    public static bool EliminaMateriale(int IdMateriale)
    {
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");
            string query = "DELETE FROM materiali WHERE IdMateriale = @IdMateriale";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@IdMateriale", IdMateriale);

            int result = command.ExecuteNonQuery();
            Console.WriteLine("Materiale eliminato con successo.");

            connessione.Close();
            return result > 0;
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
            return false;
        }
    }    

    public void AssegnaMaterialeACantiere(int cantiereId, int materialeId, int quantitàUtilizzata)
    {

        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");

            // Verifica se il materiale esiste e ha quantità sufficiente
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

                    int quantitàDisponibile = reader.GetInt32("Quantità");
                    if (quantitàDisponibile < quantitàUtilizzata)
                    {
                        throw new InvalidOperationException("Quantità insufficiente di materiale.");
                    }

                    // Crea un oggetto Materiale aggiornato
                    Materiale materialeAggiornato = new Materiale
                    {
                        IdMateriale = reader.GetInt32("IdMateriale"),
                        Nome = reader.GetString("Nome"),
                        Quantità = quantitàDisponibile - quantitàUtilizzata,
                        Unità = reader.GetString("Unità"),
                        CostoUnitario = reader.GetDouble("CostoUnitario")
                    };

                    // Chiama il metodo AggiornaMateriale
                    AggiornaMateriale(materialeAggiornato);
                }
            }

            // Assegna il materiale al cantiere
            string queryAssegnaMateriale = "INSERT INTO materialecantiere (IdCantiere, IdMateriale, QuantitàUtilizzata) VALUES (@IdCantiere, @IdMateriale, @QuantitàUtilizzata)";
            using (MySqlCommand commandAssegnaMateriale = new(queryAssegnaMateriale, connessione))
            {
                commandAssegnaMateriale.Parameters.AddWithValue("@IdCantiere", cantiereId);
                commandAssegnaMateriale.Parameters.AddWithValue("@IdMateriale", materialeId);
                commandAssegnaMateriale.Parameters.AddWithValue("@QuantitàUtilizzata", quantitàUtilizzata);
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

    public void RimuoviMaterialeDaCantiere(int cantiereId, int materialeId, int quantitàDaRimuovere)
    {
        MaterialeCantiere materialeCantiere = null;
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            Console.WriteLine("Connessione al database effettuata.");

            // Seleziona la entry dalla table materialecantiere che ha idcantiere e idmateriale corrispondente
            string queryCantiere = "SELECT Quantità FROM materialecantiere WHERE IdCantiere = @IdCantiere AND IdMateriale = @IdMateriale";
            MySqlCommand command = new(queryCantiere, connessione);     
            command.Parameters.AddWithValue("@IdCantiere", cantiereId); 
            command.Parameters.AddWithValue("@IdMateriale", materialeId);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    materialeCantiere = new MaterialeCantiere
                    {
                        IdCantiere = reader.GetInt32("IdCantiere"),
                        IdMateriale = reader.GetInt32("IdMateriale"),
                        QuantitàUtilizzata = reader.GetInt32("QuantitàUtilizzata")
                    };
                }
            }
        }
        catch (Exception eccezione)
        {
            Console.WriteLine($"Errore: {eccezione.Message}");
        }
        if (materialeCantiere == null)
        {
            throw new InvalidOperationException("Materiale non assegnato a questo cantiere.");
        }

        if (materialeCantiere.QuantitàUtilizzata < quantitàDaRimuovere)
        {
            throw new InvalidOperationException("Quantità da rimuovere superiore alla quantità utilizzata.");
        }

        materialeCantiere.QuantitàUtilizzata -= quantitàDaRimuovere;

    }    
}