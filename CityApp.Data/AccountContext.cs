using Microsoft.EntityFrameworkCore;
using CityApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using CityApp.Data.Extensions;

namespace CityApp.Data
{
    /// <summary>
    /// HOW TO ADD MIGRATION:  Add-Migration -Name {NAME_OF_YOUR_MIGRATION} -Context CityApp.Data.AccountContext
    /// </summary>
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options)
        : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<AccountUser> AccountUsers { get; set; }

        public DbSet<Vendor> Vendors { get; set; }

        public DbSet<Violation> Violations { get; set; }

        public DbSet<ViolationType> ViolationTypes { get; set; }

        public DbSet<ViolationCategory> ViolationCategorys { get; set; }

        public DbSet<Citation> Citations { get; set; }
        public DbSet<CitationPayment> CitationPayments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<CitationAttachment> CitationAttachments { get; set; }

        public DbSet<AccountUserVendor> AccountUserVendors { get; set; }

        public DbSet<Counter> Counters { get; set; }

        public DbSet<CitationComment> CitationComment { get; set; }
        public DbSet<CitationAuditLog> CitationAuditLogs { get; set; }
        public DbSet<CitationReceipt> CitationReceipts { get; set; }
        public DbSet<CitationReminder> CitationReminders { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventViolationPrice> EventViolationPrices { get; set; }
        public DbSet<EventBoundaryCoordinate> EventBoundaryCoordinates { get; set; }

        public DbSet<ViolationQuestion> ViolationQuestions { get; set; }

        public DbSet<ViolationQuestionAnswer> ViolationQuestionAnswers { get; set; }

        public DbSet<WarningQuizResponse> WarningEventRespones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region Account

            modelBuilder.Entity<Account>(entity =>
            {

            });

            modelBuilder.Entity<AccountUser>()
                .HasIndex(u => u.Email)
                .IsUnique(true);

            #endregion

            #region Citation

            modelBuilder.Entity<Citation>(entity =>
            {
                entity.Property(d => d.Latitude).HasPrecision(18, 12);
                entity.Property(d => d.Longitude).HasPrecision(18, 12);
            });

            modelBuilder.Entity<EventBoundaryCoordinate>(entity =>
            {
                entity.Property(d => d.Latitude).HasPrecision(18, 12);
                entity.Property(d => d.Longitude).HasPrecision(18, 12);
            });


            modelBuilder.Entity<Citation>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Citation>()
                 .HasIndex(u => new { u.AccountId })
                 .IsUnique(false);

            modelBuilder.Entity<Citation>()
                .HasIndex(u => new { u.Status })
                .IsUnique(false);

            modelBuilder.Entity<Citation>()
              .HasIndex(u => new { u.CreateUserId })
              .IsUnique(false);

            modelBuilder.Entity<Citation>()
              .HasIndex(u => new { u.AssignedToId })
              .IsUnique(false);

            modelBuilder.Entity<Citation>()
              .HasIndex(u => new { u.ViolationId })
              .IsUnique(false);

            ///Add unique index on CitationNumber and AccountId
            modelBuilder.Entity<Citation>()
                .HasIndex(u => new { u.CitationNumber, u.AccountId })
                .IsUnique(true);

            #endregion

            #region Citation Comment

            modelBuilder.Entity<CitationComment>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationComment>()
              .HasOne(c => c.Citation)
              .WithMany(c => c.Comments)
              .HasForeignKey(d => d.CitationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationComment>()
             .HasIndex(u => new { u.CitationId })
             .IsUnique(false);

            #endregion

            #region Citation Comment

            modelBuilder.Entity<WarningQuizResponse>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarningQuizResponse>()
              .HasOne(c => c.Citation)
              .WithMany(c => c.WarningEventResponses)
              .HasForeignKey(d => d.CitationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WarningQuizResponse>()
             .HasIndex(u => new { u.CitationId })
             .IsUnique(false);

            #endregion

            #region Citation Payments

            modelBuilder.Entity<CitationPayment>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationPayment>()
              .HasOne(c => c.Citation)
              .WithMany(c => c.Payments)
              .HasForeignKey(d => d.CitationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationPayment>()
             .HasIndex(u => new { u.CitationId })
             .IsUnique(false);

            #endregion

            #region Citation Audit Log

            modelBuilder.Entity<CitationAuditLog>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationAuditLog>()
              .HasOne(c => c.Citation)
              .WithMany(c => c.AuditLogs)
              .HasForeignKey(d => d.CitationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationAuditLog>()
             .HasIndex(u => new { u.CitationId })
             .IsUnique(false);

            #endregion

            #region Attachment

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<Attachment>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region CitationAttachment

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<CitationAttachment>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<CitationAttachment>(entity =>
            {
                entity
                    .HasOne(c => c.Citation)
                    .WithMany(c => c.Attachments)
                    .HasForeignKey(d => d.CitationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                  .HasOne(c => c.Attachment)
                  .WithMany(b => b.Citations)
                  .HasForeignKey(d => d.AttachmentId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region Counters

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<Counter>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            ///Add unique index on Name and AccountId
            modelBuilder.Entity<Counter>()
                .HasIndex(u => new { u.Name, u.AccountId })
                .IsUnique(true);

            #endregion

            #region Vendors

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<Vendor>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            ///Add unique index on Name and AccountId
            modelBuilder.Entity<Vendor>()
                .HasIndex(u => new { u.Name, u.AccountId })
                .IsUnique(true);

            #endregion

            #region Violation

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<Violation>(entity =>
            {
                entity
                    .HasIndex(cua => new { cua.CategoryId, cua.Name })
                    .IsUnique(true);

                entity
               .HasIndex(cua => new { cua.Code, cua.AccountId })
               .IsUnique(true);

                entity
                    .HasOne(c => c.Category)
                    .WithMany()
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            #region Citation Violation Questions Answers

            modelBuilder.Entity<ViolationQuestion>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViolationQuestion>()
              .HasOne(c => c.Violation)
              .WithMany(c => c.Questions)
              .HasForeignKey(d => d.ViolationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViolationQuestion>()
             .HasIndex(u => new { u.Question, u.AccountId })
             .IsUnique(true);

            modelBuilder.Entity<ViolationQuestionAnswer>()
                .HasOne(c => c.UpdateUser)
                .WithMany()
                .HasForeignKey(d => d.UpdateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViolationQuestionAnswer>()
              .HasOne(c => c.Citation)
              .WithMany(c => c.Answers)
              .HasForeignKey(d => d.CitationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViolationQuestionAnswer>()
                  .HasOne(c => c.ViolationQuestion)
                  .WithMany()
                  .HasForeignKey(d => d.ViolationQuestionId)
                  .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ViolationQuestionAnswer>()
             .HasIndex(u => new { u.CitationId, u.AccountId })
             .IsUnique(false);



            #endregion

            #endregion

            #region ViolationType
            modelBuilder.Entity<ViolationType>(entity =>
            {

            });
            #endregion

            #region ViolationCategory

            //Specify OnDelete Restrict.  We have to do this to every entity that inherits from AccountEntity.  I really hate it. So annoying 
            modelBuilder.Entity<ViolationCategory>(entity =>
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

            #region AccountUser&VendorForeignKey

            modelBuilder.Entity<AccountUserVendor>(entity =>
            {
                entity
                    .HasOne(c => c.Vendor)
                    .WithMany()
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                  .HasOne(c => c.AccountUser)
                  .WithMany()
                  .HasForeignKey(d => d.AccountUserId)
                  .OnDelete(DeleteBehavior.Restrict);
            });

            #endregion

            #region CitationReceipt

            modelBuilder.Entity<CitationReceipt>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationReceipt>()
             .HasOne(c => c.Citation)
             .WithMany()
             .HasForeignKey(d => d.CitationId)
             .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Citation Reminder

            modelBuilder.Entity<CitationReminder>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CitationReminder>()
             .HasOne(c => c.Citation)
             .WithMany()
             .HasForeignKey(d => d.CitationId)
             .OnDelete(DeleteBehavior.Restrict);

            #endregion


            #region  Event

            modelBuilder.Entity<Event>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
               .HasMany(t => t.EventViolationPrices)
               .WithOne(d => d.Event)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Event>()
               .HasMany(t => t.EventBoundaryCoordinates)
               .WithOne(d => d.Event)
               .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region EventPricing

            modelBuilder.Entity<EventViolationPrice>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventViolationPrice>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventViolationPrice>()
            .HasOne(c => c.Violation)
            .WithMany()
            .HasForeignKey(d => d.ViolationId)
            .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region EventBoundaryCoordinates

            modelBuilder.Entity<EventBoundaryCoordinate>()
               .HasOne(c => c.UpdateUser)
               .WithMany()
               .HasForeignKey(d => d.UpdateUserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EventBoundaryCoordinate>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Restrict);


            #endregion
        }
    }
}
