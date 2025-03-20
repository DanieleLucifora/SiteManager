namespace SiteManager.Models;

public class Operaio
{
    public int IdOperaio { get; set; }
    public required string Nome { get; set; }
    public required string Cognome { get; set; }
    public required string Mansione { get; set; }
    public decimal CostoOrario { get; set; }
    public DateTime DataNascita { get; set; }
    public DateTime DataAssunzione { get; set; }
    public int? CantiereId { get; set; }
    public Cantiere? Cantiere { get; set; }

    public Color BackgroundColor { get; set; } = Colors.Transparent;
}