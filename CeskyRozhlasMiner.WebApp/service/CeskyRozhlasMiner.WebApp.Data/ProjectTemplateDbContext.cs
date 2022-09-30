using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Data
{
    public class ProjectTemplateDbContext : DbContext
    {
        public ProjectTemplateDbContext(DbContextOptions<ProjectTemplateDbContext> options)
            : base(options)
        {
        }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSourceStation> PlaylistSourceStations { get; set; }

        public override int SaveChanges()
        {
            SetupAuditTrail();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetupAuditTrail();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetupAuditTrail();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetupAuditTrail();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureIndexes(modelBuilder);

            ConfigureRelationships(modelBuilder);

            ConfigureSeedData(modelBuilder);
        }

        private void SetupAuditTrail()
        {
            // automatically stamp date fields on every context save
            var dtNow = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity is AuditModel<int>)
                {
                    var entity = entry.Entity as AuditModel<int>;
                    if (entry.State == EntityState.Added)
                    {
                        entity.CreatedDate = entity.UpdatedDate = dtNow;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        entity.UpdatedDate = dtNow;
                    }
                }
            }
        }

        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Library>()
                .OwnsOne(lib => lib.Address);

            modelBuilder.Entity<Playlist>()
                .HasOne(playlist => playlist.Owner)
                .WithMany(owner => owner.Playlists)
                .HasForeignKey(playlist => playlist.OwnerId);

            modelBuilder.Entity<Playlist>()
                .OwnsMany(playlist => playlist.SourceStations);
        }

        private static void ConfigureSeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasData(
                    new Group()
                    {
                        Id = 1,
                        Name = "Surface",
                        IsActive = true
                    },
                    new Group()
                    {
                        Id = 2,
                        Name = "HoloLens",
                        IsActive = true
                    },
                    new Group()
                    {
                        Id = 3,
                        Name = "Xbox",
                        IsActive = true
                    }
                );
        }

        private static void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Library>()
                .HasIndex(x => x.Name)
                .IsUnique();

            modelBuilder.Entity<Playlist>()
                .HasIndex(x => new { x.OwnerId, x.Name })
                .IsUnique();
        }
    }
}
