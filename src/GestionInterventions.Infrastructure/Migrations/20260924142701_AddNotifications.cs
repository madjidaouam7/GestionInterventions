using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionInterventions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
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
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdentityUserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstLue = table.Column<bool>(type: "INTEGER", nullable: false),
                    DemandeId = table.Column<int>(type: "INTEGER", nullable: true),
                    InterventionId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_DateCreation",
                table: "Notifications",
                column: "DateCreation");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IdentityUserId_EstLue",
                table: "Notifications",
                columns: new[] { "IdentityUserId", "EstLue" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

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
