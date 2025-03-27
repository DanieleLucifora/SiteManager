using MySql.Data.MySqlClient;
using SiteManager.Models;

namespace SiteManager.Services
{
    public class MaterialeCantiereService
    {
        private static MySqlConnection GetConnection()
        {
            string stringaConnessione = "Server=localhost;Port=3307;Database=SiteManager;User=root;Password=1234;";
            return new MySqlConnection(stringaConnessione);
        }

        public static List<MaterialeCantiere> OttieniMaterialeCantiere(int idCantiere)
        {
            var materialiUtilizzati = new List<MaterialeCantiere>();

            try
            {
                var connessione = GetConnection();
                connessione.Open();
                Console.WriteLine("Connessione al database effettuata.");

                string query = @"
                    SELECT mc.IdMaterialeCantiere, mc.IdCantiere, mc.IdMateriale, mc.QuantitaUtilizzata, 
                        m.Nome AS MaterialeNome, m.CostoUnitario, m.Quantita, m.Unita
                    FROM materialecantiere mc
                    JOIN materiali m ON mc.IdMateriale = m.IdMateriale
                    WHERE mc.IdCantiere = @IdCantiere";

                MySqlCommand command = new MySqlCommand(query, connessione);
                command.Parameters.AddWithValue("@IdCantiere", idCantiere);
                                
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    materialiUtilizzati.Add(new MaterialeCantiere
                    {
                        IdMaterialeCantiere = reader.GetInt32("IdMaterialeCantiere"),
                        IdCantiere = reader.GetInt32("IdCantiere"),
                        IdMateriale = reader.GetInt32("IdMateriale"),
                        QuantitaUtilizzata = reader.GetInt32("QuantitaUtilizzata"),
                        Materiale = new Materiale
                        {
                            IdMateriale = reader.GetInt32("IdMateriale"),
                            Nome = reader.GetString("MaterialeNome"),
                            CostoUnitario = reader.GetDouble("CostoUnitario"),
                            Quantita = reader.GetInt32("Quantita"),
                            Unita = reader.GetString("Unita")
                        }
                    });
                }

                reader.Close();
                connessione.Close();
                return materialiUtilizzati;
            }
            catch (Exception eccezione)
            {
                Console.WriteLine($"Errore: {eccezione.Message}");
                return materialiUtilizzati;
            }
        }
    }
}