using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigrationName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Rejete",
                table: "VersionRFQs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Valide",
                table: "VersionRFQs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Rejete",
                table: "RFQs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Valide",
                table: "RFQs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rejete",
                table: "VersionRFQs");

            migrationBuilder.DropColumn(
                name: "Valide",
                table: "VersionRFQs");

            migrationBuilder.DropColumn(
                name: "Rejete",
                table: "RFQs");

            migrationBuilder.DropColumn(
                name: "Valide",
                table: "RFQs");
        }
    }
}
