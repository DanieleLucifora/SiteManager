namespace SiteManager.Models;

public class Presenza
{
    public int IdPresenza { get; set; }
    public int? OperaioId { get; set; }
    public decimal Ore { get; set; }
    public DateTime Data { get; set; }
    public int? CantiereId { get; set; }
    public Operaio? Operaio { get; set; }
    public Cantiere? Cantiere { get; set; }
}
