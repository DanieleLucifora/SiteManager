using System;

namespace SiteManager.Models;

public class Materiale
{
    public int IdMateriale { get; set; }
    public required string Nome { get; set; }
    public required int Quantità { get; set; }
    public required string Unità { get; set; }
    public required double CostoUnitario { get; set; }
    public List<MaterialeCantiere> materialiCantiere = [];
}