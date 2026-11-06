using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatOverlord.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeuilDeblocage = table.Column<double>(type: "float", nullable: false),
                    ThemeFront = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Joueurs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pseudo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MotDePasseHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DerniereMiseAJour = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NombreClics = table.Column<long>(type: "bigint", nullable: false),
                    NombrePrestiges = table.Column<int>(type: "int", nullable: false),
                    PalierActuelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Joueurs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Joueurs_Paliers_PalierActuelId",
                        column: x => x.PalierActuelId,
                        principalTable: "Paliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ressources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PalierDeblocageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ressources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ressources_Paliers_PalierDeblocageId",
                        column: x => x.PalierDeblocageId,
                        principalTable: "Paliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JoueurRessources",
                columns: table => new
                {
                    JoueurId = table.Column<int>(type: "int", nullable: false),
                    RessourceId = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<double>(type: "float", nullable: false),
                    ProductionParSeconde = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoueurRessources", x => new { x.JoueurId, x.RessourceId });
                    table.ForeignKey(
                        name: "FK_JoueurRessources_Joueurs_JoueurId",
                        column: x => x.JoueurId,
                        principalTable: "Joueurs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoueurRessources_Ressources_RessourceId",
                        column: x => x.RessourceId,
                        principalTable: "Ressources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoueurRessources_RessourceId",
                table: "JoueurRessources",
                column: "RessourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Joueurs_Email",
                table: "Joueurs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Joueurs_PalierActuelId",
                table: "Joueurs",
                column: "PalierActuelId");

            migrationBuilder.CreateIndex(
                name: "IX_Joueurs_Pseudo",
                table: "Joueurs",
                column: "Pseudo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ressources_Code",
                table: "Ressources",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ressources_PalierDeblocageId",
                table: "Ressources",
                column: "PalierDeblocageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoueurRessources");

            migrationBuilder.DropTable(
                name: "Joueurs");

            migrationBuilder.DropTable(
                name: "Ressources");

            migrationBuilder.DropTable(
                name: "Paliers");
        }
    }
}
