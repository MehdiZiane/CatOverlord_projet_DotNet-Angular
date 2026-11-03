// CatOverlord.Domain/Entities/Palier.cs
namespace CatOverlord.Domain.Entities;

public class Palier
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double SeuilDeblocage { get; set; }
    public string ThemeFront { get; set; } = string.Empty;

    public ICollection<Ressource> Ressources { get; set; } = new List<Ressource>();
}