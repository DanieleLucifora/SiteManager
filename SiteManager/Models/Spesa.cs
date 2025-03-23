namespace SiteManager.Models;

public class Spesa
{
    public int IdSpesa { get; set; }
    public required string Descrizione { get; set; }
    public DateTime Data { get; set; }
    public decimal Costo { get; set; }
    public int? CantiereId { get; set; }
    public Cantiere? Cantiere { get; set; }
}
