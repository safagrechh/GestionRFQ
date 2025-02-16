﻿// <auto-generated />
using System;
using EX.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EX.Data.Migrations
{
    [DbContext(typeof(EXContext))]
    [Migration("20250216134929_UpdateDeleteBehaviors2")]
    partial class UpdateDeleteBehaviors2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EX.Core.Domain.Client", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sales")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("EX.Core.Domain.Commentaire", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenu")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateC")
                        .HasColumnType("datetime2");

                    b.Property<int?>("RFQId")
                        .HasColumnType("int");

                    b.Property<int>("ValidateurId")
                        .HasColumnType("int");

                    b.Property<int?>("VersionRFQId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RFQId");

                    b.HasIndex("ValidateurId");

                    b.HasIndex("VersionRFQId");

                    b.ToTable("Commentaires");
                });

            modelBuilder.Entity("EX.Core.Domain.HistoriqueAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CibleAction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAction")
                        .HasColumnType("datetime2");

                    b.Property<string>("DetailsAction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReferenceCible")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("HistoriqueActions");
                });

            modelBuilder.Entity("EX.Core.Domain.MarketSegment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MarketSegments");
                });

            modelBuilder.Entity("EX.Core.Domain.RFQ", b =>
                {
                    b.Property<int>("CodeRFQ")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodeRFQ"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CDDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ClientId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CustomerDataDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstV")
                        .HasColumnType("int");

                    b.Property<int?>("IngenieurRFQId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("KODate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LRDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MRDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MarketSegmentId")
                        .HasColumnType("int");

                    b.Property<int?>("MaterialLeaderId")
                        .HasColumnType("int");

                    b.Property<int>("MaxV")
                        .HasColumnType("int");

                    b.Property<int>("NumRefQuoted")
                        .HasColumnType("int");

                    b.Property<string>("QuoteName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("SOPDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Statut")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TRDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TestLeaderId")
                        .HasColumnType("int");

                    b.Property<int?>("ValidateurId")
                        .HasColumnType("int");

                    b.HasKey("CodeRFQ");

                    b.HasIndex("ClientId");

                    b.HasIndex("IngenieurRFQId");

                    b.HasIndex("MarketSegmentId");

                    b.HasIndex("MaterialLeaderId");

                    b.HasIndex("TestLeaderId");

                    b.HasIndex("ValidateurId");

                    b.ToTable("RFQs");
                });

            modelBuilder.Entity("EX.Core.Domain.Rapport", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CheminFichier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Rapports");
                });

            modelBuilder.Entity("EX.Core.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomUser")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("EX.Core.Domain.VersionRFQ", b =>
                {
                    b.Property<int>("CodeV")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CodeV"));

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("CustomerDataDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("datetime2");

                    b.Property<int>("EstV")
                        .HasColumnType("int");

                    b.Property<int?>("IngenieurRFQId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("KODate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("LRDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("MRDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MarketSegmentId")
                        .HasColumnType("int");

                    b.Property<int?>("MaterialLeaderId")
                        .HasColumnType("int");

                    b.Property<int>("MaxV")
                        .HasColumnType("int");

                    b.Property<int>("NumRefQuoted")
                        .HasColumnType("int");

                    b.Property<string>("QuoteName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RFQId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("SOPDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Statut")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TDDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TRDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("TestLeaderId")
                        .HasColumnType("int");

                    b.Property<int?>("ValidateurId")
                        .HasColumnType("int");

                    b.HasKey("CodeV");

                    b.HasIndex("IngenieurRFQId");

                    b.HasIndex("MarketSegmentId");

                    b.HasIndex("MaterialLeaderId");

                    b.HasIndex("RFQId");

                    b.HasIndex("TestLeaderId");

                    b.HasIndex("ValidateurId");

                    b.ToTable("VersionRFQs");
                });

            modelBuilder.Entity("EX.Core.Domain.Worker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Workers");
                });

            modelBuilder.Entity("EX.Core.Domain.Commentaire", b =>
                {
                    b.HasOne("EX.Core.Domain.RFQ", "RFQ")
                        .WithMany("Commentaires")
                        .HasForeignKey("RFQId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("EX.Core.Domain.User", "Validateur")
                        .WithMany("Commentaires")
                        .HasForeignKey("ValidateurId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EX.Core.Domain.VersionRFQ", "VersionRFQ")
                        .WithMany("Commentaires")
                        .HasForeignKey("VersionRFQId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("RFQ");

                    b.Navigation("Validateur");

                    b.Navigation("VersionRFQ");
                });

            modelBuilder.Entity("EX.Core.Domain.HistoriqueAction", b =>
                {
                    b.HasOne("EX.Core.Domain.User", "User")
                        .WithMany("HistoriqueActions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("EX.Core.Domain.RFQ", b =>
                {
                    b.HasOne("EX.Core.Domain.Client", "Client")
                        .WithMany("RFQs")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.User", "IngenieurRFQ")
                        .WithMany("RFQsEnTantQueIngenieur")
                        .HasForeignKey("IngenieurRFQId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.MarketSegment", "MarketSegment")
                        .WithMany("RFQs")
                        .HasForeignKey("MarketSegmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.Worker", "MaterialLeader")
                        .WithMany("AsMaterialLeader")
                        .HasForeignKey("MaterialLeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.Worker", "TestLeader")
                        .WithMany("AsTestLeader")
                        .HasForeignKey("TestLeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.User", "Validateur")
                        .WithMany("RFQsEnTantQueValidateur")
                        .HasForeignKey("ValidateurId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Client");

                    b.Navigation("IngenieurRFQ");

                    b.Navigation("MarketSegment");

                    b.Navigation("MaterialLeader");

                    b.Navigation("TestLeader");

                    b.Navigation("Validateur");
                });

            modelBuilder.Entity("EX.Core.Domain.Rapport", b =>
                {
                    b.HasOne("EX.Core.Domain.User", "user")
                        .WithMany("Rapports")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("user");
                });

            modelBuilder.Entity("EX.Core.Domain.VersionRFQ", b =>
                {
                    b.HasOne("EX.Core.Domain.User", "IngenieurRFQ")
                        .WithMany("VersionRFQsEnTantQueIngenieur")
                        .HasForeignKey("IngenieurRFQId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.MarketSegment", "MarketSegment")
                        .WithMany("Versions")
                        .HasForeignKey("MarketSegmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.Worker", "MaterialLeader")
                        .WithMany("VMaterialLeader")
                        .HasForeignKey("MaterialLeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.RFQ", "RFQ")
                        .WithMany("Versions")
                        .HasForeignKey("RFQId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("EX.Core.Domain.Worker", "TestLeader")
                        .WithMany("VTestLeader")
                        .HasForeignKey("TestLeaderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("EX.Core.Domain.User", "Validateur")
                        .WithMany("VersionRFQsEnTantQueValidateur")
                        .HasForeignKey("ValidateurId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("IngenieurRFQ");

                    b.Navigation("MarketSegment");

                    b.Navigation("MaterialLeader");

                    b.Navigation("RFQ");

                    b.Navigation("TestLeader");

                    b.Navigation("Validateur");
                });

            modelBuilder.Entity("EX.Core.Domain.Client", b =>
                {
                    b.Navigation("RFQs");
                });

            modelBuilder.Entity("EX.Core.Domain.MarketSegment", b =>
                {
                    b.Navigation("RFQs");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("EX.Core.Domain.RFQ", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("EX.Core.Domain.User", b =>
                {
                    b.Navigation("Commentaires");

                    b.Navigation("HistoriqueActions");

                    b.Navigation("RFQsEnTantQueIngenieur");

                    b.Navigation("RFQsEnTantQueValidateur");

                    b.Navigation("Rapports");

                    b.Navigation("VersionRFQsEnTantQueIngenieur");

                    b.Navigation("VersionRFQsEnTantQueValidateur");
                });

            modelBuilder.Entity("EX.Core.Domain.VersionRFQ", b =>
                {
                    b.Navigation("Commentaires");
                });

            modelBuilder.Entity("EX.Core.Domain.Worker", b =>
                {
                    b.Navigation("AsMaterialLeader");

                    b.Navigation("AsTestLeader");

                    b.Navigation("VMaterialLeader");

                    b.Navigation("VTestLeader");
                });
#pragma warning restore 612, 618
        }
    }
}
