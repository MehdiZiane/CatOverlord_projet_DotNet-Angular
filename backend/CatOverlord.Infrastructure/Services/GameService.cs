using CatOverlord.Application.DTOs;
using CatOverlord.Application.Interfaces;
using CatOverlord.Domain.Entities;
using CatOverlord.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CatOverlord.Infrastructure.Services;

public class GameService : IGameService
{
    private const string PseudoTemporaire = "joueur-temp";
    private const double GainParClic = 1;

    private readonly AppDbContext _db;

    public GameService(AppDbContext db) => _db = db;

    public async Task<GameStateDto> GetStateAsync()
    {
        var joueur = await GetOrCreateJoueurTemporaireAsync();
        return ToDto(joueur);
    }

    public async Task<GameStateDto> ClickAsync()
    {
        var joueur = await GetOrCreateJoueurTemporaireAsync();

        var croquettes = joueur.Ressources.First(r => r.Ressource.Code == "croquettes");
        croquettes.Quantite += GainParClic;
        joueur.NombreClics++;
        joueur.DerniereMiseAJour = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return ToDto(joueur);
    }

    private async Task<Joueur> GetOrCreateJoueurTemporaireAsync()
    {
        var joueur = await _db.Joueurs
            .Include(j => j.PalierActuel)
            .Include(j => j.Ressources).ThenInclude(jr => jr.Ressource)
            .FirstOrDefaultAsync(j => j.Pseudo == PseudoTemporaire);

        if (joueur is not null) return joueur;

        var palier = await _db.Paliers.SingleAsync(p => p.Numero == 1);
        var croquettes = await _db.Ressources.SingleAsync(r => r.Code == "croquettes");

        joueur = new Joueur
        {
            Pseudo = PseudoTemporaire,
            Email = "temp@catoverlord.local",
            MotDePasseHash = "TEMPORAIRE",
            DateCreation = DateTime.UtcNow,
            DerniereMiseAJour = DateTime.UtcNow,
            PalierActuel = palier
        };
        joueur.Ressources.Add(new JoueurRessource { Ressource = croquettes, Quantite = 0, ProductionParSeconde = 0 });

        _db.Joueurs.Add(joueur);
        await _db.SaveChangesAsync();
        return joueur;
    }

    private static GameStateDto ToDto(Joueur j) => new(
        j.Pseudo,
        j.NombreClics,
        j.PalierActuel.Nom,
        j.PalierActuel.ThemeFront,
        j.Ressources
            .Select(r => new RessourceEtatDto(r.Ressource.Code, r.Ressource.Nom, r.Ressource.Icone, r.Quantite, r.ProductionParSeconde))
            .ToList());
}