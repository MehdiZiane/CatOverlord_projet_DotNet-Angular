namespace CatOverlord.Domain.Entities;

public class Joueur
{
   public int Id { get; set; }
   public string Pseudo{ get; set; } = string.Empty;
   public string Email{ get; set; } = string.Empty;
   public string MotDePasseHash{ get; set; } = string.Empty;
   public DateTime DateCreation{ get; set; }
   public DateTime DerniereMiseAJour{ get; set; }
   public long NombreClics{ get; set; }
   public int NombrePrestiges{ get; set; }
   public int PalierActuelId{ get; set; }
   public Palier PalierActuel{ get; set; } = null!;
   public ICollection<JoueurRessource> Ressources{ get; set; } = new List<JoueurRessource>();
}