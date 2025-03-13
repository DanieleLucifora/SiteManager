namespace SiteManager.Models;

public class Materiale
{
    public int IdMateriale { get; set; }
    public required string Nome { get; set; }
    public required int Quantita { get; set; }
    public required string Unita { get; set; }
    public required double CostoUnitario { get; set; }
}