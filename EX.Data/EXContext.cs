using EX.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace EX.Data
{
    public class EXContext:DbContext
    {
        public DbSet<RFQ> RFQs { get; set; }
        public DbSet<VersionRFQ> VersionRFQs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<MarketSegment> MarketSegments { get; set; }
        public DbSet<HistoriqueAction> HistoriqueActions { get; set; }
        public DbSet<Commentaire> Commentaires { get; set; }
        public DbSet<Rapport> Rapports { get; set; }

        public EXContext(DbContextOptions<EXContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // RFQ -> IngenieurRFQ
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.IngenieurRFQ)
                .WithMany(u => u.RFQsEnTantQueIngenieur)
                .HasForeignKey(r => r.IngenieurRFQId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> Validateur
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.Validateur)
                .WithMany(u => u.RFQsEnTantQueValidateur)
                .HasForeignKey(r => r.ValidateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> MaterialLeader (Worker)
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.MaterialLeader)
                .WithMany(w => w.AsMaterialLeader) // No inverse navigation property, so leave empty
                .HasForeignKey(r => r.MaterialLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> TestLeader (Worker)
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.TestLeader)
                .WithMany(w => w.AsTestLeader)
                .HasForeignKey(r => r.TestLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> MarketSegment
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.MarketSegment)
                .WithMany(m => m.RFQs)
                .HasForeignKey(r => r.MarketSegmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> Client
            modelBuilder.Entity<RFQ>()
                .HasOne(r => r.Client)
                .WithMany(c => c.RFQs)
                .HasForeignKey(r => r.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // VersionRFQ -> IngenieurRFQ (User)
            modelBuilder.Entity<VersionRFQ>()
                .HasOne(v => v.IngenieurRFQ)
                .WithMany(u => u.VersionRFQsEnTantQueIngenieur)
                .HasForeignKey(v => v.IngenieurRFQId)
                .OnDelete(DeleteBehavior.Restrict);

            // VersionRFQ -> Validateur (User)
            modelBuilder.Entity<VersionRFQ>()
                .HasOne(v => v.Validateur)
                .WithMany(u => u.VersionRFQsEnTantQueValidateur)
                .HasForeignKey(v => v.ValidateurId)
                .OnDelete(DeleteBehavior.Restrict);

            // VersionRFQ -> MaterialLeader (Worker)
            modelBuilder.Entity<VersionRFQ>()
                .HasOne(r => r.MaterialLeader)
                .WithMany(w => w.VMaterialLeader) // No inverse navigation property, so leave empty
                .HasForeignKey(r => r.MaterialLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // VersionRFQ -> TestLeader (Worker)
            modelBuilder.Entity<VersionRFQ>()
                .HasOne(r => r.TestLeader)
                .WithMany(w => w.VTestLeader)
                .HasForeignKey(r => r.TestLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> MarketSegment
            modelBuilder.Entity<VersionRFQ>()
                .HasOne(r => r.MarketSegment)
                .WithMany(m => m.Versions)
                .HasForeignKey(r => r.MarketSegmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // RFQ -> Versions (one-to-many)
            modelBuilder.Entity<RFQ>()
                .HasMany(r => r.Versions)
                .WithOne(v => v.RFQ)
                .HasForeignKey(v => v.RFQId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade delete versions when RFQ is deleted

            // RFQ -> Commentaires (one-to-many)
            modelBuilder.Entity<RFQ>()
                .HasMany(r => r.Commentaires)
                .WithOne(c => c.RFQ)
                .HasForeignKey(c => c.RFQId)
                .OnDelete(DeleteBehavior.Cascade); // Disable cascade delete for RFQ -> Commentaire

            // VersionRFQ -> Commentaires (one-to-many)
            modelBuilder.Entity<VersionRFQ>()
                .HasMany(v => v.Commentaires)
                .WithOne(c => c.VersionRFQ)
                .HasForeignKey(c => c.VersionRFQId)
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete comments when VersionRFQ is deleted
        }

    }
}