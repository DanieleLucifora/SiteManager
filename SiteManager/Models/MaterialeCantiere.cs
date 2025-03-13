namespace SiteManager.Models;

public class MaterialeCantiere
{
    public int IdMaterialeCantiere { get; set; }
    public int IdCantiere { get; set; }  
    public int IdMateriale { get; set; }
    public int QuantitaUtilizzata { get; set; }
    public Cantiere? Cantiere { get; set; }  
    public Materiale? Materiale { get; set; }
}