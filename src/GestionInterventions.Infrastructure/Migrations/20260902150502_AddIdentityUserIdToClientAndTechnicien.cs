using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionInterventions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityUserIdToClientAndTechnicien : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Techniciens",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Interventions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Techniciens");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Clients");

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
