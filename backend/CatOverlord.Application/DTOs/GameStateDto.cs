namespace CatOverlord.Application.DTOs;

public record RessourceEtatDto(string Code, string Nom, string Icone, double Quantite, double ProductionParSeconde);

public record GameStateDto(
    string Pseudo,
    long NombreClics,
    string PalierNom,
    string PalierTheme,
    IReadOnlyList<RessourceEtatDto> Ressources);