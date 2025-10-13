using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class createdbyuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "RFQs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_CreatedByUserId",
                table: "RFQs",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_Users_CreatedByUserId",
                table: "RFQs",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_Users_CreatedByUserId",
                table: "RFQs");

            migrationBuilder.DropIndex(
                name: "IX_RFQs_CreatedByUserId",
                table: "RFQs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RFQs");
        }
    }
}
