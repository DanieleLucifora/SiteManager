using MySql.Data.MySqlClient;
using SiteManager.Models;

using System;

namespace SiteManager.Services;

public class SpesaService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    

    public static List<Spesa> OttieniSpese(Cantiere cantiere)
    {
        List<Spesa> spese = [];
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = $"SELECT * FROM spesecantiere WHERE CantiereId = '{cantiere.IdCantiere}'";
            MySqlCommand command = new(query, connessione); 
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                spese.Add(new Spesa
                {
                    IdSpesa = reader.GetInt32("IdSpesa"),
                    Descrizione = reader.GetString("Descrizione"),
                    Data = reader.GetDateTime("Data"),
                    Costo = reader.GetDecimal("Costo"),
                    CantiereId = reader.GetInt32("CantiereId")
                });
            }
            return spese;
        }
        catch (Exception)
        {
            return spese;
        }
    } 

    public static bool AggiungiSpesa(Spesa spesa)
    {
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO spesecantiere (Descrizione, Data, Costo, CantiereId) VALUES (@Descrizione, @Data, @Costo, @CantiereId)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@Descrizione", spesa.Descrizione);
            command.Parameters.AddWithValue("@Data", DateTime.Now);
            command.Parameters.AddWithValue("@Costo", spesa.Costo);
            command.Parameters.AddWithValue("@CantiereId", spesa.CantiereId);

            int result = command.ExecuteNonQuery();
            if (result > 0) 
            {
                spesa.IdSpesa = (int)command.LastInsertedId;
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
}
