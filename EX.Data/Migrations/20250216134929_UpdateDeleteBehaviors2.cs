using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commentaires_VersionRFQs_VersionRFQId",
                table: "Commentaires");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaires_VersionRFQs_VersionRFQId",
                table: "Commentaires",
                column: "VersionRFQId",
                principalTable: "VersionRFQs",
                principalColumn: "CodeV",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commentaires_VersionRFQs_VersionRFQId",
                table: "Commentaires");

            migrationBuilder.AddForeignKey(
                name: "FK_Commentaires_VersionRFQs_VersionRFQId",
                table: "Commentaires",
                column: "VersionRFQId",
                principalTable: "VersionRFQs",
                principalColumn: "CodeV",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
