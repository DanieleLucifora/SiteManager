namespace SiteManager.Models;

public class Cantiere
{
    public int IdCantiere { get; set; }
    public required string Citta { get; set; }
    public required string Committente { get; set; }
    public DateTime DataInizio { get; set; }
    public DateTime Scadenza { get; set; }
    public List<Operaio> operai = [];
    public List<Materiale> materiali = [];
    public List<Task> tasks = [];
}