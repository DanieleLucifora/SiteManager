using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace SiteManager.Models;

public class DataContext
{
    private readonly string stringaConnessione = "Server=localhost;Database=SiteManager;User=root;Password=1234;";

    //Costruttore che stabilisce la connessione con il db Sitemanager
    public DataContext()
    {
        MySqlConnection connessione = new(stringaConnessione);
        connessione.Open();
        Console.WriteLine("Connessione al database effettuata.");
    }

}