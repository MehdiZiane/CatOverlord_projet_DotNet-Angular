using CatOverlord.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CatOverlord.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Joueur> Joueurs => Set<Joueur>();
    public DbSet<Palier> Paliers => Set<Palier>();
    public DbSet<Ressource> Ressources => Set<Ressource>();
    public DbSet<JoueurRessource> JoueurRessources => Set<JoueurRessource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Joueur>(e =>
        {
            e.Property(j => j.Pseudo).HasMaxLength(50);
            e.Property(j => j.Email).HasMaxLength(256);
            e.HasIndex(j => j.Pseudo).IsUnique();
            e.HasIndex(j => j.Email).IsUnique();

            e.HasOne(j => j.PalierActuel)
             .WithMany()
             .HasForeignKey(j => j.PalierActuelId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ressource>(e =>
        {
            e.Property(r => r.Code).HasMaxLength(50);
            e.HasIndex(r => r.Code).IsUnique();

            e.HasOne(r => r.PalierDeblocage)
             .WithMany(p => p.Ressources)
             .HasForeignKey(r => r.PalierDeblocageId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<JoueurRessource>(e =>
        {
            e.HasKey(jr => new { jr.JoueurId, jr.RessourceId });

            e.HasOne(jr => jr.Joueur)
             .WithMany(j => j.Ressources)
             .HasForeignKey(jr => jr.JoueurId);

            e.HasOne(jr => jr.Ressource)
             .WithMany()
             .HasForeignKey(jr => jr.RessourceId);
        });
    }
}