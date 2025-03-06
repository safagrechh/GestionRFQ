using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class VALeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_Users_ValidateurId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionRFQs_Users_ValidateurId",
                table: "VersionRFQs");

            migrationBuilder.RenameColumn(
                name: "ValidateurId",
                table: "VersionRFQs",
                newName: "VALeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_VersionRFQs_ValidateurId",
                table: "VersionRFQs",
                newName: "IX_VersionRFQs_VALeaderId");

            migrationBuilder.RenameColumn(
                name: "ValidateurId",
                table: "RFQs",
                newName: "VALeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_RFQs_ValidateurId",
                table: "RFQs",
                newName: "IX_RFQs_VALeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_Users_VALeaderId",
                table: "RFQs",
                column: "VALeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionRFQs_Users_VALeaderId",
                table: "VersionRFQs",
                column: "VALeaderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_Users_VALeaderId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionRFQs_Users_VALeaderId",
                table: "VersionRFQs");

            migrationBuilder.RenameColumn(
                name: "VALeaderId",
                table: "VersionRFQs",
                newName: "ValidateurId");

            migrationBuilder.RenameIndex(
                name: "IX_VersionRFQs_VALeaderId",
                table: "VersionRFQs",
                newName: "IX_VersionRFQs_ValidateurId");

            migrationBuilder.RenameColumn(
                name: "VALeaderId",
                table: "RFQs",
                newName: "ValidateurId");

            migrationBuilder.RenameIndex(
                name: "IX_RFQs_VALeaderId",
                table: "RFQs",
                newName: "IX_RFQs_ValidateurId");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_Users_ValidateurId",
                table: "RFQs",
                column: "ValidateurId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionRFQs_Users_ValidateurId",
                table: "VersionRFQs",
                column: "ValidateurId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
