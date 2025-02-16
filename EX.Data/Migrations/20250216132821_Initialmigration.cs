using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EX.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sales = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketSegments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistoriqueActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CibleAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReferenceCible = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsAction = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAction = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoriqueActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoriqueActions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rapports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheminFichier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rapports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rapports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RFQs",
                columns: table => new
                {
                    CodeRFQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumRefQuoted = table.Column<int>(type: "int", nullable: false),
                    SOPDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxV = table.Column<int>(type: "int", nullable: false),
                    EstV = table.Column<int>(type: "int", nullable: false),
                    KODate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerDataDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    MaterialLeaderId = table.Column<int>(type: "int", nullable: true),
                    TestLeaderId = table.Column<int>(type: "int", nullable: true),
                    MarketSegmentId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    IngenieurRFQId = table.Column<int>(type: "int", nullable: true),
                    ValidateurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RFQs", x => x.CodeRFQ);
                    table.ForeignKey(
                        name: "FK_RFQs_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RFQs_MarketSegments_MarketSegmentId",
                        column: x => x.MarketSegmentId,
                        principalTable: "MarketSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RFQs_Users_IngenieurRFQId",
                        column: x => x.IngenieurRFQId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RFQs_Users_ValidateurId",
                        column: x => x.ValidateurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RFQs_Workers_MaterialLeaderId",
                        column: x => x.MaterialLeaderId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RFQs_Workers_TestLeaderId",
                        column: x => x.TestLeaderId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VersionRFQs",
                columns: table => new
                {
                    CodeV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuoteName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumRefQuoted = table.Column<int>(type: "int", nullable: false),
                    SOPDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxV = table.Column<int>(type: "int", nullable: false),
                    EstV = table.Column<int>(type: "int", nullable: false),
                    KODate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerDataDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LRDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CDDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    MaterialLeaderId = table.Column<int>(type: "int", nullable: true),
                    TestLeaderId = table.Column<int>(type: "int", nullable: true),
                    MarketSegmentId = table.Column<int>(type: "int", nullable: false),
                    RFQId = table.Column<int>(type: "int", nullable: false),
                    IngenieurRFQId = table.Column<int>(type: "int", nullable: true),
                    ValidateurId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionRFQs", x => x.CodeV);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_MarketSegments_MarketSegmentId",
                        column: x => x.MarketSegmentId,
                        principalTable: "MarketSegments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_RFQs_RFQId",
                        column: x => x.RFQId,
                        principalTable: "RFQs",
                        principalColumn: "CodeRFQ",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_Users_IngenieurRFQId",
                        column: x => x.IngenieurRFQId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_Users_ValidateurId",
                        column: x => x.ValidateurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_Workers_MaterialLeaderId",
                        column: x => x.MaterialLeaderId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VersionRFQs_Workers_TestLeaderId",
                        column: x => x.TestLeaderId,
                        principalTable: "Workers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commentaires",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contenu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidateurId = table.Column<int>(type: "int", nullable: false),
                    RFQId = table.Column<int>(type: "int", nullable: true),
                    VersionRFQId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commentaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commentaires_RFQs_RFQId",
                        column: x => x.RFQId,
                        principalTable: "RFQs",
                        principalColumn: "CodeRFQ",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commentaires_Users_ValidateurId",
                        column: x => x.ValidateurId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commentaires_VersionRFQs_VersionRFQId",
                        column: x => x.VersionRFQId,
                        principalTable: "VersionRFQs",
                        principalColumn: "CodeV",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commentaires_RFQId",
                table: "Commentaires",
                column: "RFQId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaires_ValidateurId",
                table: "Commentaires",
                column: "ValidateurId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaires_VersionRFQId",
                table: "Commentaires",
                column: "VersionRFQId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoriqueActions_UserId",
                table: "HistoriqueActions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Rapports_UserId",
                table: "Rapports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_ClientId",
                table: "RFQs",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_IngenieurRFQId",
                table: "RFQs",
                column: "IngenieurRFQId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_MarketSegmentId",
                table: "RFQs",
                column: "MarketSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_MaterialLeaderId",
                table: "RFQs",
                column: "MaterialLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_TestLeaderId",
                table: "RFQs",
                column: "TestLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_RFQs_ValidateurId",
                table: "RFQs",
                column: "ValidateurId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_IngenieurRFQId",
                table: "VersionRFQs",
                column: "IngenieurRFQId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_MarketSegmentId",
                table: "VersionRFQs",
                column: "MarketSegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_MaterialLeaderId",
                table: "VersionRFQs",
                column: "MaterialLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_RFQId",
                table: "VersionRFQs",
                column: "RFQId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_TestLeaderId",
                table: "VersionRFQs",
                column: "TestLeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionRFQs_ValidateurId",
                table: "VersionRFQs",
                column: "ValidateurId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commentaires");

            migrationBuilder.DropTable(
                name: "HistoriqueActions");

            migrationBuilder.DropTable(
                name: "Rapports");

            migrationBuilder.DropTable(
                name: "VersionRFQs");

            migrationBuilder.DropTable(
                name: "RFQs");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "MarketSegments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Workers");
        }
    }
}
