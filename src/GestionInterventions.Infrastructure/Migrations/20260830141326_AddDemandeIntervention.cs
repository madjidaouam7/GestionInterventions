using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionInterventions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDemandeIntervention : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DemandeInterventions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DateDemande = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EquipementId = table.Column<int>(type: "INTEGER", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    Priorite = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandeInterventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DemandeInterventions_Equipements_EquipementId",
                        column: x => x.EquipementId,
                        principalTable: "Equipements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DemandeInterventions_EquipementId",
                table: "DemandeInterventions",
                column: "EquipementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DemandeInterventions");
        }
    }
}
