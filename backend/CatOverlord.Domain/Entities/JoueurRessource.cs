namespace CatOverlord.Domain.Entities;

public class JoueurRessource
{
    public int JoueurId { get; set; }
    public Joueur Joueur { get; set; } = null!;
    public int RessourceId { get; set; }
    public Ressource Ressource { get; set; } = null!;
    public double Quantite { get; set; }
    public double ProductionParSeconde { get; set; }
}