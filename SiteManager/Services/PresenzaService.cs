using MySql.Data.MySqlClient;
using SiteManager.Models;

using System;

namespace SiteManager.Services;

public class PresenzaService
{
    private static MySqlConnection GetConnection()
    {
        string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
        return new MySqlConnection(stringaConnessione);
    }    

    public static List<Presenza> OttieniPresenze(Cantiere cantiere)
    {
        List<Presenza> presenze = [];
        try
        {
            var connessione = GetConnection();
            connessione.Open();
            string query = $"SELECT * FROM presenzecantiere WHERE CantiereId = '{cantiere.IdCantiere}'";
            MySqlCommand command = new(query, connessione); 
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                presenze.Add(new Presenza
                {
                    IdPresenza = reader.GetInt32("IdPresenza"),
                    OperaioId = reader.GetInt32("OperaioId"),
                    Ore = reader.GetDecimal("Ore"),
                    Data = reader.GetDateTime("Data"),
                    CantiereId = reader.GetInt32("CantiereId")
                });
            }
            return presenze;
        }
        catch (Exception)
        {
            return presenze;
        }
    }    

    public static bool AggiungiPresenza(Presenza presenza)
    {
        try
        {
            MySqlConnection connessione = GetConnection();
            connessione.Open();
            string query = "INSERT INTO presenzecantiere (OperaioId, Ore, Data, CantiereId) VALUES (@OperaioId, @Ore, @Data, @CantiereId)";
            MySqlCommand command = new(query, connessione);

            command.Parameters.AddWithValue("@OperaioId", presenza.OperaioId);
            command.Parameters.AddWithValue("@Ore", presenza.Ore);
            command.Parameters.AddWithValue("@Data", DateTime.Now);
            command.Parameters.AddWithValue("@CantiereId", presenza.CantiereId);

            int result = command.ExecuteNonQuery();
            if (result > 0) 
            {
                presenza.IdPresenza = (int)command.LastInsertedId;    
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
