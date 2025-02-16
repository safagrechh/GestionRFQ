using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_Clients_ClientId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_MarketSegments_MarketSegmentId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionRFQs_MarketSegments_MarketSegmentId",
                table: "VersionRFQs");

            migrationBuilder.AlterColumn<int>(
                name: "MarketSegmentId",
                table: "VersionRFQs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MarketSegmentId",
                table: "RFQs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "RFQs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_Clients_ClientId",
                table: "RFQs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_MarketSegments_MarketSegmentId",
                table: "RFQs",
                column: "MarketSegmentId",
                principalTable: "MarketSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionRFQs_MarketSegments_MarketSegmentId",
                table: "VersionRFQs",
                column: "MarketSegmentId",
                principalTable: "MarketSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_Clients_ClientId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_RFQs_MarketSegments_MarketSegmentId",
                table: "RFQs");

            migrationBuilder.DropForeignKey(
                name: "FK_VersionRFQs_MarketSegments_MarketSegmentId",
                table: "VersionRFQs");

            migrationBuilder.AlterColumn<int>(
                name: "MarketSegmentId",
                table: "VersionRFQs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MarketSegmentId",
                table: "RFQs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ClientId",
                table: "RFQs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_Clients_ClientId",
                table: "RFQs",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RFQs_MarketSegments_MarketSegmentId",
                table: "RFQs",
                column: "MarketSegmentId",
                principalTable: "MarketSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VersionRFQs_MarketSegments_MarketSegmentId",
                table: "VersionRFQs",
                column: "MarketSegmentId",
                principalTable: "MarketSegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
