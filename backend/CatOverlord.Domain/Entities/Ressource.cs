// CatOverlord.Domain/Entities/Ressource.cs
namespace CatOverlord.Domain.Entities;

public class Ressource
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;   // ex : "croquettes"
    public string Nom { get; set; } = string.Empty;
    public string Icone { get; set; } = string.Empty;  // un emoji

    public int PalierDeblocageId { get; set; }
    public Palier PalierDeblocage { get; set; } = null!;
}