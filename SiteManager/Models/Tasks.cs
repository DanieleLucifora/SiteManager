namespace SiteManager.Models;

public class Tasks
{
    public int IdTasks { get; set; }
    public required string Descrizione { get; set; }
    public DateTime Data { get; set; }
    public int? CantiereId { get; set; }
    public Cantiere? Cantiere { get; set; } 
}