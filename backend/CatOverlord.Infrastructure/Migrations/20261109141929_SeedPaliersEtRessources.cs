using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatOverlord.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPaliersEtRessources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Paliers",
                columns: new[] { "Id", "Description", "Nom", "Numero", "SeuilDeblocage", "ThemeFront" },
                values: new object[,]
                {
                    { 1, "Un canapé, un rayon de soleil et des humains très naïfs.", "Le salon", 1, 0.0, "theme-salon" },
                    { 2, "Les chats errants ont compris qu'ils pouvaient travailler pour toi. Contre des croquettes, évidemment.", "La rue", 2, 1000.0, "theme-rue" },
                    { 3, "Le maire est allergique aux chats. Tu t'en occupes personnellement.", "La ville", 3, 100000.0, "theme-ville" },
                    { 4, "Les humains pensent encore diriger. Ils ont tort, mais c'est mignon.", "La planète", 4, 10000000.0, "theme-planete" },
                    { 5, "Prochaine cible : la Lune, qui est clairement une grosse boule de croquettes.", "L'espace", 5, 1000000000.0, "theme-espace" }
                });

            migrationBuilder.InsertData(
                table: "Ressources",
                columns: new[] { "Id", "Code", "Icone", "Nom", "PalierDeblocageId" },
                values: new object[] { 1, "croquettes", "🍖", "Croquettes", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Paliers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Paliers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Paliers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Paliers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ressources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Paliers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
