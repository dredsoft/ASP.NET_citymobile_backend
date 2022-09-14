using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CityApp.Data.Models;
using CityApp.Data.Extensions;

namespace CityApp.Data
{
    /// <summary>
    /// HOW TO ADD MIGRATION:  Add-Migration -Name {NAME_OF_YOUR_MIGRATION} -Context CityApp.Data.CommonContext
    /// </summary>
    public class CommonContext : DbContext
    {
        public CommonContext(DbContextOptions<CommonContext> options)
          : base(options)
        { }

        public DbSet<CommonUser> Users { get; set; }

        public DbSet<CommonAccount> CommonAccounts { get; set; }

        public DbSet<CommonAccountSettings> CommonAccountSettings { get; set; }

        public DbSet<CommonUserAccount> UserAccounts { get; set; }

        public DbSet<Partition> Partitions { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<CommonViolationType> CommonViolationTypes { get; set; }

        public DbSet<CommonViolationCategory> CommonViolationCategories { get; set; }

        public DbSet<CommonViolation> CommonViolations { get; set; }

        public DbSet<CommonAccountViolationType> CommonAccountViolationTypes { get; set; }

        public DbSet<CommonUserDevice> CommonUserDevices { get; set; }

        public DbSet<State> States { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region IdForeignKey
            modelBuilder.Entity<CommonAccount>(entity =>
            {
                // CommonAccount -> CommonAccountSettings one-to-one relationship, where CommonAccountSettings.Id 
                //   is both the PK and an FK back to CommonAccount.Id.
                entity
                    .HasOne(ca => ca.Settings)
                    .WithOne()
                    .HasForeignKey<CommonAccountSettings>(cas => cas.Id)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasIndex(ca => ca.Number)
                    .IsUnique(true);

            });
            #endregion

            #region UserIdForeignKey
            modelBuilder.Entity<CommonUserAccount>(entity =>
            {
                entity
                    .HasIndex(cua => new { cua.UserId, cua.AccountId })
                    .IsUnique(true);

                entity
                    .HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region Cities
            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(d => d.Latitude).HasPrecision(18, 6);
                entity.Property(d => d.Longitude).HasPrecision(18, 6);
            });
            #endregion

            #region CommonViolationCategory
            modelBuilder.Entity<CommonViolationCategory>(entity =>
            {
                entity
                    .HasIndex(cua => new { cua.TypeId, cua.Name })
                    .IsUnique(true);

                entity
                    .HasOne(c => c.Type)
                    .WithMany()
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region CategoryIdForeignKey
            modelBuilder.Entity<CommonViolation>(entity =>
            {
                entity
                    .HasIndex(cua => new { cua.CategoryId, cua.Name })
                    .IsUnique(true);

                entity
                    .HasOne(c => c.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

            #region ViolationTypeId&AccountIdForeignKey
            modelBuilder.Entity<CommonAccountViolationType>(entity =>
            {
                entity
                    .HasOne(c => c.ViolationType)
                    .WithMany()
                    .HasForeignKey(d => d.ViolationTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                  .HasOne(c => c.Account)
                  .WithMany()
                  .HasForeignKey(d => d.AccountId)
                  .OnDelete(DeleteBehavior.Restrict);
            });
            #endregion

        }
    }
}
