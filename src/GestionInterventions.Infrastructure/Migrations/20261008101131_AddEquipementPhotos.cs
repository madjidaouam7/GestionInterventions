using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionInterventions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipementPhotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Interventions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateTable(
                name: "EquipementPhoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomFichier = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CheminFichier = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DateAjout = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EquipementId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipementPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipementPhoto_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipementPhoto_EquipementId",
                table: "EquipementPhoto",
                column: "EquipementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipementPhoto");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Interventions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
