# PLAN.md : CatOverlord (suivi de l'avancement)

## Contexte

**Projet** : Cat Overlord, un jeu incrémental (idle game) inspiré d'Universal Paperclips, dans lequel un chat conquiert le monde. Projet de fin de module en formation de développeur.

**Contraintes imposées** : back-end .NET (C#) avec SQL Server, front-end Angular + TypeScript, peu de tables mais de vraies relations, POO visible (héritage, polymorphisme, interfaces), **minimum 25 endpoints** (objectif 33).

**Stack et versions**
- .NET SDK : **10.0.400**, cible `net10.0` (migré depuis .NET 8)
- EF Core : **10.0.x** (SqlServer et Design en `10.0.*`, résolu en 10.0.14), outil `dotnet-ef` 10.0.x en outil local
- Base de données : SQL Server **LocalDB**, instance `MSSQLLocalDB`, base `CatOverlordDb`
- Front : Angular CLI 22.2.4, Node 26.8.1 (Angular 22 exige Node >= 22.12), non démarré
- Outils : VS Code, SSMS 22, Git (dépôt GitHub `MehdiZiane/CatOverlord_projet_DotNet-Angular`)
- Shell : PowerShell et Git Bash sous Windows

**Structure réelle des dossiers**
```
CatOverlord_projet_DotNet-Angular/
  global.json                      (SDK 10.0.400, rollForward latestFeature)
  .gitignore
  backend/
    CatOverlord.sln
    .config/dotnet-tools.json      (dotnet-ef 10.0.x)
    CatOverlord.Domain/            (entités, ne dépend de rien)
      Entities/
    CatOverlord.Application/       (services métier, interfaces)
    CatOverlord.Infrastructure/    (EF Core, DbContext, migrations)
      Persistence/AppDbContext.cs
      Migrations/
    CatOverlord.Api/               (controllers, Program.cs, appsettings.json)
    CatOverlord.Tests/             (xUnit)
  frontend/                        (à créer, Angular)
```

**Graphe de dépendances attendu**
```
Api            -> Application, Infrastructure
Infrastructure -> Application, Domain
Application    -> Domain
Tests          -> Application, Domain
Domain         -> (rien)
```

**Conventions**
- Propriétés et classes publiques en PascalCase. Noms de domaine en français (Joueur, Palier, Ressource), singulier pour les classes, pluriel pour les `DbSet` et les tables.
- Une classe par fichier, nom du fichier identique à celui de la classe, extension `.cs` en minuscules.
- Dates toujours en **UTC** (`DateTime.UtcNow`, jamais `DateTime.Now`).
- Quantités de jeu en `double`.
- Pas de secret dans le dépôt (la chaîne de connexion LocalDB n'a pas de mot de passe ; la future clé JWT ira dans `dotnet user-secrets`).
- Le serveur fait foi pour les calculs de jeu.

**Plan par tranches verticales**
1. Chat cliquable : base minimale, `GET state`, `POST click`, projet Angular et page d'accueil
2. Boutique : `Amelioration` en héritage TPH, `IAchetable`, achat, tests xUnit
3. Authentification : JWT, rôles, remplacement du joueur temporaire
4. Production par seconde, tick local, sync, gains hors-ligne
5. Paliers : passage de palier, thème, messages ironiques
6. CRUD admin pour atteindre les 25 endpoints, finitions, crédits

Dette technique assumée en tranche 1 : un joueur temporaire sans connexion, à remplacer en tranche 3.

---

## Backend

### B1. Verrouillage du SDK
- [ ] ✅ `global.json` (racine du dépôt)
  - Doit contenir : `sdk.version` = `10.0.400`, `sdk.rollForward` = `latestFeature`. JSON valide.
  - Vérifier : `dotnet --version` depuis la racine affiche `10.0.400`.

### B2. Solution et projets
- [ ] ✅ `backend/CatOverlord.sln` contient 5 projets : Api, Application, Domain, Infrastructure, Tests
  - Vérifier : `dotnet sln list` (depuis `backend`) liste les 5 `.csproj`.
- [ ] ✅ Les 5 `.csproj` ont `<TargetFramework>net10.0</TargetFramework>`
  - Vérifier : `Select-String -Path */*.csproj -Pattern "TargetFramework"` (aucun `net8.0`).
- [ ] ✅ Références de projets conformes au graphe de dépendances (7 références)
  - Vérifier : `dotnet list <projet> reference` pour chaque projet. Le Domain n'en a aucune.
- [ ] Aucun `<InvariantGlobalization>true</InvariantGlobalization>` dans les `.csproj` (provoque `Globalization Invariant Mode is not supported` avec SqlClient)
  - Vérifier : `Select-String -Path */*.csproj -Pattern "Invariant"` ne renvoie rien.

### B3. Paquets NuGet et outil EF
- [ ] ✅ `backend/CatOverlord.Infrastructure/CatOverlord.Infrastructure.csproj` : `Microsoft.EntityFrameworkCore.SqlServer` version `10.0.*`
- [ ] ✅ `backend/CatOverlord.Api/CatOverlord.Api.csproj` : `Microsoft.EntityFrameworkCore.Design` version `10.0.*`
- [ ] ✅ `backend/.config/dotnet-tools.json` : `dotnet-ef` en `10.0.*`
  - Vérifier : `dotnet list CatOverlord.Infrastructure package`, `dotnet ef --version` affiche `10.0.x`.
- [ ] Avertissements connus et acceptés pour l'instant : `NU1902` et `NU1903` sur `System.Security.Cryptography.Pkcs 9.0.11` (dépendance transitive). À traiter en fin de projet (`dotnet list package --include-transitive`).

### B4. Entités du Domain (`backend/CatOverlord.Domain/Entities/`)
- [ ] ✅ `Palier.cs` : `Id` (int), `Numero` (int), `Nom`, `Description`, `ThemeFront` (string, défaut `string.Empty`), `SeuilDeblocage` (double), `ICollection<Ressource> Ressources` (initialisée en `new List<Ressource>()`)
- [ ] ✅ `Ressource.cs` : `Id`, `Code`, `Nom`, `Icone` (string), `PalierDeblocageId` (int), `PalierDeblocage` (Palier, `= null!`)
  - Vérifier : le fichier s'appelle `Ressource.cs` (singulier) et correspond à la classe `Ressource`.
- [ ] ✅ `Joueur.cs` : `Id`, `Pseudo` (majuscule), `Email`, `MotDePasseHash` (string), `DateCreation`, `DerniereMiseAJour` (DateTime), `NombreClics` (long), `NombrePrestiges` (int), `PalierActuelId` (int), `PalierActuel` (Palier, `= null!`), `ICollection<JoueurRessource> Ressources` (initialisée en `new List<JoueurRessource>()`)
- [ ] ✅ `JoueurRessource.cs` : `JoueurId`, `Joueur` (navigation), `RessourceId`, `Ressource` (navigation), `Quantite` (double), `ProductionParSeconde` (double)
  - Vérifier : nom de fichier `JoueurRessource.cs` sans faute de frappe ; les deux propriétés de navigation sont présentes.
  - Vérifier pour l'ensemble : `dotnet build` depuis `backend` finit avec **0 erreur**.

### B5. DbContext (`backend/CatOverlord.Infrastructure/Persistence/AppDbContext.cs`)
- [ ] ✅ Classe `AppDbContext : DbContext`, constructeur `AppDbContext(DbContextOptions<AppDbContext> options)`
- [ ] ✅ `DbSet` : `Joueurs`, `Paliers`, `Ressources`, `JoueurRessources`
- [ ] ✅ `OnModelCreating` :
  - `Joueur` : `Pseudo` `HasMaxLength(50)`, `Email` `HasMaxLength(256)`, index uniques sur `Pseudo` et `Email`, relation vers `PalierActuel` avec `DeleteBehavior.Restrict`
  - `Ressource` : `Code` `HasMaxLength(50)` + index unique, relation vers `PalierDeblocage` (`WithMany(p => p.Ressources)`) avec `DeleteBehavior.Restrict`
  - `JoueurRessource` : clé composite `HasKey(jr => new { jr.JoueurId, jr.RessourceId })`, relations vers `Joueur` (`WithMany(j => j.Ressources)`) et `Ressource`
- [ ] Nettoyage : supprimer `Class1.cs` dans Infrastructure et Application (fichiers générés inutiles)

### B6. API : configuration (`backend/CatOverlord.Api/`)
- [ ] ✅ `appsettings.json` : JSON valide (virgule après `"AllowedHosts": "*"`), section `ConnectionStrings:DefaultConnection` = `Server=(localdb)\\MSSQLLocalDB;Database=CatOverlordDb;Trusted_Connection=True;TrustServerCertificate=True`
  - Vérifier : aucune erreur de syntaxe JSON dans VS Code (onglet Problems).
- [ ] ✅ `Program.cs` : `using CatOverlord.Infrastructure.Persistence;` et `using Microsoft.EntityFrameworkCore;`, puis `builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));` avant `AddControllers()`
- [ ] Nettoyage : supprimer `WeatherForecast.cs` et `Controllers/WeatherForecastController.cs` (modèle par défaut inutile)

### B7. Tranche 1 : état du jeu et clic
- [x] DTO `RessourceEtatDto` et `GameStateDto` présents dans `CatOverlord.Application/DTOs/GameStateDto.cs` avec les champs attendus.
- [x] `IGameService` déclare `GetStateAsync()` et `ClickAsync()`, retournant `Task<GameStateDto>`.
- [x] `GameService : IGameService` injecte `AppDbContext`; le joueur temporaire `joueur-temp` est créé avec le palier 1 et une ligne de ressource `croquettes`.
- [x] `ClickAsync` ajoute une croquette, incrémente `NombreClics`, fixe `DerniereMiseAJour` avec `DateTime.UtcNow` et appelle `SaveChangesAsync`.
- [x] `Include` / `ThenInclude` chargent `PalierActuel` et `Ressource` avant la conversion en DTO.
- [x] `GameController` expose `GET api/game/state` et `POST api/game/click`, retourne `ActionResult<GameStateDto>` et délègue à `IGameService`.
- [x] `Program.cs` enregistre `IGameService` avec `GameService` via `AddScoped`.
- [x] Architecture vérifiée : Application référence seulement Domain et ne référence ni EF Core ni Infrastructure; le contrôleur ne retourne que des DTO.
- [x] Nettoyage vérifié : les deux `Class1.cs` et les fichiers `WeatherForecast.cs` / `WeatherForecastController.cs` sont supprimés.
- [ ] `dotnet build` standard depuis `backend` : échec temporaire car la DLL Debug est verrouillée par le processus `CatOverlord.Api` (PID 15900). Vérification alternative réussie avec `dotnet build -c Release` (0 erreur; avertissements NU1902/NU1903 connus).
- [x] Aucun fichier `bin/` ou `obj/` n'apparaît dans `git status`.

---

## Base de données

### D1. Migration initiale
- [ ] ✅ Dossier `backend/CatOverlord.Infrastructure/Migrations/` contenant `20261106153523_InitialCreate.cs`, son `.Designer.cs` et `AppDbContextModelSnapshot.cs`
  - Doit contenir : `CreateTable` pour `Paliers`, `Joueurs`, `Ressources`, `JoueurRessources` ; clé primaire `PK_JoueurRessources` sur (`JoueurId`, `RessourceId`) ; index uniques `IX_Joueurs_Email`, `IX_Joueurs_Pseudo`, `IX_Ressources_Code`.
  - Commande : `dotnet ef migrations add InitialCreate --project CatOverlord.Infrastructure --startup-project CatOverlord.Api` (depuis `backend`)
- [ ] ✅ Migration appliquée
  - Commande : `dotnet ef database update --project CatOverlord.Infrastructure --startup-project CatOverlord.Api`
  - Vérifier dans SSMS (serveur `(localdb)\MSSQLLocalDB`, authentification Windows, certificat de confiance coché) : base `CatOverlordDb` avec les tables `Joueurs`, `Paliers`, `Ressources`, `JoueurRessources`, `__EFMigrationsHistory`.

### D2. Seed : paliers et ressource (À FAIRE)
- [ ] Dans `AppDbContext.OnModelCreating`, à la fin, ajouter `modelBuilder.Entity<Palier>().HasData(...)` avec 5 paliers (Id explicites) :

| Id / Numero | Nom | SeuilDeblocage | ThemeFront |
|---|---|---|---|
| 1 | Le salon | 0 | `theme-salon` |
| 2 | La rue | 1000 | `theme-rue` |
| 3 | La ville | 100000 | `theme-ville` |
| 4 | La planète | 10000000 | `theme-planete` |
| 5 | L'espace | 1000000000 | `theme-espace` |

  Chaque palier a une `Description` humoristique non vide.
- [ ] `modelBuilder.Entity<Ressource>().HasData(new Ressource { Id = 1, Code = "croquettes", Nom = "Croquettes", Icone = "🍖", PalierDeblocageId = 1 })`
- [ ] Nouvelle migration `SeedPaliersEtRessources` (`dotnet ef migrations add SeedPaliersEtRessources ...`) contenant des `InsertData` pour `Paliers` et `Ressources`, puis `dotnet ef database update ...`
  - Vérifier en SQL : `SELECT * FROM Paliers;` renvoie 5 lignes ; `SELECT * FROM Ressources;` renvoie 1 ligne (`croquettes`, `PalierDeblocageId = 1`, emoji affiché correctement).

---

## Frontend

- [ ] Non démarré. Projet Angular à créer dans `frontend/` (composants standalone, signals), puis page d'accueil, écran de jeu avec le gros bouton « ronronner », appels à l'API. Détail des tâches à ajouter quand cette étape sera donnée.

---

## Autres

### A1. Dépôt Git
- [ ] ✅ Dépôt initialisé, branche `main`, remote `origin` = `https://github.com/MehdiZiane/CatOverlord_projet_DotNet-Angular.git`
- [ ] ✅ Passage à .NET 10 commité et poussé
  - Vérifier : `git status` propre, `git log --oneline` montre les commits ; aucun dossier `bin/` ou `obj/` suivi.

### A2. Tests
- [ ] `backend/CatOverlord.Tests/` (xUnit, `net10.0`) existe et référence Application et Domain. Aucun test métier écrit pour l'instant (`UnitTest1.cs` est le fichier généré). Tests à venir en tranche 2.
  - Vérifier : `dotnet test` s'exécute sans erreur.

### A3. Environnement
- [ ] ✅ LocalDB présent : `sqllocaldb info` affiche `MSSQLLocalDB`
- [ ] Pour tout nouveau poste : cloner, vérifier le SDK 10, puis `dotnet ef database update ...` recrée la base (migrations + seed).

---

## Instructions pour Copilot

1. Lis intégralement ce fichier.
2. Compare chaque tâche avec le code réel du workspace : ouvre les fichiers indiqués, vérifie les noms de classes, propriétés, chemins et valeurs de configuration, et lance les commandes de vérification quand c'est possible.
3. Réponds par un tableau unique :

| Tâche | Statut | Détail |
|---|---|---|
| (identifiant et intitulé) | OK / manquant / incorrect | Pour « manquant » ou « incorrect » : fichier, ligne, ce qui est attendu et la correction précise à faire |

4. Ne modifie aucun fichier sans que je le demande. Signale aussi les erreurs de conception (relations, comportements de suppression, types), pas seulement la syntaxe.
5. Termine par la liste des écarts à corriger, classés par priorité.