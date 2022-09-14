using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CityApp.Data;
using CityApp.Data.Enums;

namespace CityApp.Data.Migrations
{
    [DbContext(typeof(CommonContext))]
    [Migration("20170907155027_AccountFeature")]
    partial class AccountFeature
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CityApp.Data.Models.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("County")
                        .HasMaxLength(100);

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,6)");

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("State")
                        .HasMaxLength(50);

                    b.Property<string>("StateCode")
                        .HasMaxLength(2);

                    b.Property<string>("TimeZone")
                        .HasMaxLength(100);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Type")
                        .HasMaxLength(25);

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address1")
                        .HasMaxLength(250);

                    b.Property<string>("Address2")
                        .HasMaxLength(250);

                    b.Property<bool>("Archived");

                    b.Property<Guid?>("AttachmentId");

                    b.Property<int>("CitationWorkflow");

                    b.Property<Guid>("CityId");

                    b.Property<string>("CityName")
                        .HasMaxLength(200);

                    b.Property<string>("ContactEmail")
                        .HasMaxLength(255);

                    b.Property<string>("ContactNumber")
                        .HasMaxLength(15);

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<int>("Features");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<long>("Number");

                    b.Property<Guid?>("OwnerUserId");

                    b.Property<Guid>("PartitionId");

                    b.Property<string>("State")
                        .HasMaxLength(50);

                    b.Property<string>("StorageBucketName")
                        .HasMaxLength(100);

                    b.Property<bool>("Suspended");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<string>("Zip")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.HasIndex("Number")
                        .IsUnique();

                    b.HasIndex("OwnerUserId");

                    b.HasIndex("PartitionId");

                    b.ToTable("CommonAccounts");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccountSettings", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.ToTable("CommonAccountSettings");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccountViolationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<Guid?>("ViolationTypeId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("ViolationTypeId");

                    b.ToTable("CommonAccountViolationTypes");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<DateTime?>("FailedResetUtc");

                    b.Property<int>("FailedSinceResetCount");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .HasMaxLength(50);

                    b.Property<string>("LastSession")
                        .HasMaxLength(50);

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50);

                    b.Property<string>("Password")
                        .HasMaxLength(100);

                    b.Property<int>("Permission");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(15);

                    b.Property<string>("ProfileImageKey")
                        .HasMaxLength(255);

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("Token")
                        .HasMaxLength(100);

                    b.Property<DateTime?>("TokenUtc");

                    b.Property<string>("TotpRecovery")
                        .HasMaxLength(50);

                    b.Property<string>("TotpSecret")
                        .HasMaxLength(100);

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonUserAccount", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<bool>("Disabled");

                    b.Property<DateTime?>("ExpirationUtc");

                    b.Property<int>("Permissions");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<Guid?>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("UserId", "AccountId")
                        .IsUnique();

                    b.ToTable("UserAccounts");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonUserDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<string>("DeviceName")
                        .HasMaxLength(100);

                    b.Property<string>("DevicePublicKey")
                        .HasMaxLength(500);

                    b.Property<string>("DeviceToken")
                        .HasMaxLength(500);

                    b.Property<string>("DeviceType")
                        .HasMaxLength(100);

                    b.Property<bool>("IsDisabled");

                    b.Property<bool>("IsLogin");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("CommonUserDevices");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonViolation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Actions");

                    b.Property<Guid?>("CategoryId");

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

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

                    b.HasIndex("CategoryId", "Name")
                        .IsUnique();

                    b.ToTable("CommonViolations");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonViolationCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

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

                    b.HasIndex("TypeId", "Name")
                        .IsUnique();

                    b.ToTable("CommonViolationCategories");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonViolationType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

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

                    b.ToTable("CommonViolationTypes");
                });

            modelBuilder.Entity("CityApp.Data.Models.Partition", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConnectionString")
                        .IsRequired()
                        .HasMaxLength(1024);

                    b.Property<Guid>("CreateUserId");

                    b.Property<DateTime>("CreateUtc");

                    b.Property<bool>("Disabled");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<long>("Occupancy");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("UpdateUserId");

                    b.Property<DateTime>("UpdateUtc");

                    b.HasKey("Id");

                    b.ToTable("Partitions");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccount", b =>
                {
                    b.HasOne("CityApp.Data.Models.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.CommonUser", "OwnerUser")
                        .WithMany()
                        .HasForeignKey("OwnerUserId");

                    b.HasOne("CityApp.Data.Models.Partition", "Partition")
                        .WithMany()
                        .HasForeignKey("PartitionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccountSettings", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonAccount")
                        .WithOne("Settings")
                        .HasForeignKey("CityApp.Data.Models.CommonAccountSettings", "Id");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonAccountViolationType", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId");

                    b.HasOne("CityApp.Data.Models.CommonViolationType", "ViolationType")
                        .WithMany()
                        .HasForeignKey("ViolationTypeId");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonUserAccount", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonAccount", "Account")
                        .WithMany()
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CityApp.Data.Models.CommonUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonUserDevice", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonViolation", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonViolationCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("CityApp.Data.Models.CommonViolationCategory", b =>
                {
                    b.HasOne("CityApp.Data.Models.CommonViolationType", "Type")
                        .WithMany()
                        .HasForeignKey("TypeId");
                });
        }
    }
}
