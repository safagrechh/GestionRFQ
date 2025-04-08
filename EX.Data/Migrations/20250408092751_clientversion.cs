using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class clientversion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "VersionRFQs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_ClientId",
                table: "VersionRFQs",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionRFQs_Clients_ClientId",
                table: "VersionRFQs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionRFQs_Clients_ClientId",
                table: "VersionRFQs");

            migrationBuilder.DropIndex(
                name: "IX_VersionRFQs_ClientId",
                table: "VersionRFQs");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "VersionRFQs");
        }
    }
}
