using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CityApp.Data;
using CityApp.Data.Enums;

namespace CityApp.Data.Migrations.Account
{
    [DbContext(typeof(AccountContext))]
    [Migration("20170804130846_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CityApp.Data.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("CityApp.Data.Models.AccountUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("FirstName")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .HasMaxLength(50);

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("AccountUsers");
                });

            modelBuilder.Entity("CityApp.Data.Models.AccountUserVendor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountUserId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<Guid>("VendorId");

                    b.HasKey("Id");

                    b.HasIndex("AccountUserId");

                    b.HasIndex("VendorId");

                    b.ToTable("AccountUserVendors");
                });

            modelBuilder.Entity("CityApp.Data.Models.Attachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<long>("ContentLength");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("MimeType")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("UpdateUserId");

                    b.ToTable("Attachments");
                });

            modelBuilder.Entity("CityApp.Data.Models.Citation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid?>("AssignedToId");

                    b.Property<long>("CitationNumber");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("LicensePlate")
                        .HasMaxLength(25);

                    b.Property<string>("LocationDescription")
                        .HasMaxLength(255);

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,6)");

                    b.Property<int>("Status");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<string>("VehicleColor")
                        .HasMaxLength(50);

                    b.Property<string>("VehicleMake")
                        .HasMaxLength(50);

                    b.Property<string>("VehicleModel")
                        .HasMaxLength(50);

                    b.Property<string>("VehicleType")
                        .HasMaxLength(50);

                    b.Property<Guid?>("ViolationId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AssignedToId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("Status");

                    b.HasIndex("UpdateUserId");

                    b.HasIndex("ViolationId");

                    b.HasIndex("CitationNumber", "AccountId")
                       .IsUnique();

                    b.ToTable("Citations");
                });

            modelBuilder.Entity("CityApp.Data.Models.CitationAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("AttachmentId");

                    b.Property<Guid>("CitationId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AttachmentId");

                    b.HasIndex("CitationId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("UpdateUserId");

                    b.ToTable("CitationAttachments");
                });

            modelBuilder.Entity("CityApp.Data.Models.CitationComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("CitationId");

                    b.Property<string>("Comment")
                        .HasMaxLength(500);

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<bool>("IsPublic");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CitationId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("UpdateUserId");

                    b.ToTable("CitationComment");
                });

            modelBuilder.Entity("CityApp.Data.Models.Counter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<long>("NextValue");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("UpdateUserId");

                    b.HasIndex("Name", "AccountId")
                        .IsUnique();

                    b.ToTable("Counters");
                });

            modelBuilder.Entity("CityApp.Data.Models.Vendor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<int>("Actions");

                    b.Property<string>("Address1")
                        .HasMaxLength(250);

                    b.Property<string>("Address2")
                        .HasMaxLength(250);

                    b.Property<string>("City")
                        .HasMaxLength(200);

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<bool>("Disabled");

                    b.Property<string>("Email");

                    b.Property<string>("MobilePhone")
                        .HasMaxLength(15);

                    b.Property<string>("Name")
                        .HasMaxLength(150);

                    b.Property<string>("OfficePhone")
                        .HasMaxLength(15);

                    b.Property<string>("State")
                        .HasMaxLength(50);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<string>("Zip");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("CreateUserId");

                    b.HasIndex("UpdateUserId");

                    b.HasIndex("Name", "AccountId")
                        .IsUnique();

                    b.ToTable("Vendors");
                });

            modelBuilder.Entity("CityApp.Data.Models.Violation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<int>("Actions");

                    b.Property<Guid?>("CategoryId");

                    b.Property<string>("Code");

                    b.Property<Guid?>("CommonViolationId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<int>("CustomActions");

                    b.Property<string>("CustomDescription")
                        .HasMaxLength(500);

                    b.Property<string>("CustomHelpUrl")
                        .HasMaxLength(250);

                    b.Property<string>("CustomName")
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<bool>("Disabled");

                    b.Property<string>("HelpUrl")
                        .HasMaxLength(250);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.HasIndex("CategoryId", "Name")
                        .IsUnique();

                    b.ToTable("Violations");
                });

            modelBuilder.Entity("CityApp.Data.Models.ViolationCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid?>("CommonCategoryId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("CustomDescription")
                        .HasMaxLength(500);

                    b.Property<string>("CustomName")
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<bool>("Disabled");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("TypeId");

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("TypeId", "Name")
                        .IsUnique();

                    b.ToTable("ViolationCategorys");
                });

            modelBuilder.Entity("CityApp.Data.Models.ViolationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("CommonViolationTypeId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("CustomDescription")
                        .HasMaxLength(500);

                    b.Property<string>("CustomName")
                        .HasMaxLength(100);

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<bool>("Disabled");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("ViolationTypes");
                });

            modelBuilder.Entity("CityApp.Data.Models.AccountUserVendor", b =>
                {
                    b.HasOne("CityApp.Data.Models.AccountUser", "AccountUser")
                        .WithMany()
                        .HasForeignKey("AccountUserId");

                    b.HasOne("CityApp.Data.Models.Vendor", "Vendor")
                        .WithMany()
                        .HasForeignKey("VendorId");
                });

            modelBuilder.Entity("CityApp.Data.Models.Attachment", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.Citation", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "AssignedTo")
                        .WithMany()
                        .HasForeignKey("AssignedToId");

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");

                    b.HasOne("CityApp.Data.Models.Violation", "Violation")
                        .WithMany()
                        .HasForeignKey("ViolationId");
                });

            modelBuilder.Entity("CityApp.Data.Models.CitationAttachment", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.Attachment", "Attachment")
                        .WithMany("Citations")
                        .HasForeignKey("AttachmentId");

                    b.HasOne("CityApp.Data.Models.Citation", "Citation")
                        .WithMany("Attachments")
                        .HasForeignKey("CitationId");

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.CitationComment", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.Citation", "Citation")
                        .WithMany("Comments")
                        .HasForeignKey("CitationId");

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.Counter", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.Vendor", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "CreateUser")
                        .WithMany()
                        .HasForeignKey("CreateUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.AccountUser", "UpdateUser")
                        .WithMany()
                        .HasForeignKey("UpdateUserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.Violation", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.ViolationCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("CityApp.Data.Models.ViolationCategory", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.ViolationType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });

            modelBuilder.Entity("CityApp.Data.Models.ViolationType", b =>
                {
                    b.HasOne("CityApp.Data.Models.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
