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

        modelBuilder.Entity<Palier>().HasData(
            new Palier
            {
                Id = 1,
                Numero = 1,
                Nom = "Le salon",
                Description = "Un canapé, un rayon de soleil et des humains très naïfs.",
                SeuilDeblocage = 0,
                ThemeFront = "theme-salon"
            },
            new Palier
            {
                Id = 2,
                Numero = 2,
                Nom = "La rue",
                Description = "Les chats errants ont compris qu'ils pouvaient travailler pour toi. Contre des croquettes, évidemment.",
                SeuilDeblocage = 1000,
                ThemeFront = "theme-rue"
            },
            new Palier
            {
                Id = 3,
                Numero = 3,
                Nom = "La ville",
                Description = "Le maire est allergique aux chats. Tu t'en occupes personnellement.",
                SeuilDeblocage = 100000,
                ThemeFront = "theme-ville"
            },
            new Palier
            {
                Id = 4,
                Numero = 4,
                Nom = "La planète",
                Description = "Les humains pensent encore diriger. Ils ont tort, mais c'est mignon.",
                SeuilDeblocage = 10000000,
                ThemeFront = "theme-planete"
            },
            new Palier
            {
                Id = 5,
                Numero = 5,
                Nom = "L'espace",
                Description = "Prochaine cible : la Lune, qui est clairement une grosse boule de croquettes.",
                SeuilDeblocage = 1000000000,
                ThemeFront = "theme-espace"
            }
        );

        modelBuilder.Entity<Ressource>().HasData(
            new Ressource
            {
                Id = 1,
                Code = "croquettes",
                Nom = "Croquettes",
                Icone = "🍖",
                PalierDeblocageId = 1
            }
        );
    }
}