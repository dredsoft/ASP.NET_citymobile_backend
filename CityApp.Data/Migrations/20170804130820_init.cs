using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    County = table.Column<string>(maxLength: 100, nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    State = table.Column<string>(maxLength: 50, nullable: true),
                    StateCode = table.Column<string>(maxLength: 2, nullable: true),
                    TimeZone = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Type = table.Column<string>(maxLength: 25, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    FailedResetUtc = table.Column<DateTime>(nullable: true),
                    FailedSinceResetCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    LastSession = table.Column<string>(maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 50, nullable: true),
                    Password = table.Column<string>(maxLength: 100, nullable: true),
                    Permission = table.Column<int>(nullable: false),
                    PhoneNumber = table.Column<string>(maxLength: 15, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Token = table.Column<string>(maxLength: 100, nullable: true),
                    TokenUtc = table.Column<DateTime>(nullable: true),
                    TotpRecovery = table.Column<string>(maxLength: 50, nullable: true),
                    TotpSecret = table.Column<string>(maxLength: 100, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonViolationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Disabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonViolationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Partitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConnectionString = table.Column<string>(maxLength: 1024, nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Occupancy = table.Column<long>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonUserDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    DeviceName = table.Column<string>(maxLength: 100, nullable: true),
                    DeviceToken = table.Column<string>(maxLength: 500, nullable: true),
                    DeviceType = table.Column<string>(maxLength: 100, nullable: true),
                    IsDisabled = table.Column<bool>(nullable: false),
                    IsLogin = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonUserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonUserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonViolationCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Disabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TypeId = table.Column<Guid>(nullable: false),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonViolationCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonViolationCategories_CommonViolationTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "CommonViolationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Archived = table.Column<bool>(nullable: false),
                    CitationWorkflow = table.Column<int>(nullable: false),
                    CityId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Number = table.Column<long>(nullable: false),
                    OwnerUserId = table.Column<Guid>(nullable: true),
                    PartitionId = table.Column<Guid>(nullable: false),
                    StorageBucketName = table.Column<string>(maxLength: 100, nullable: true),
                    Suspended = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAccounts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonAccounts_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommonAccounts_Partitions_PartitionId",
                        column: x => x.PartitionId,
                        principalTable: "Partitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonViolations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Actions = table.Column<int>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Disabled = table.Column<bool>(nullable: false),
                    HelpUrl = table.Column<string>(maxLength: 250, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonViolations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonViolations_CommonViolationCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "CommonViolationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonAccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAccountSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAccountSettings_CommonAccounts_Id",
                        column: x => x.Id,
                        principalTable: "CommonAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonAccountViolationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    ViolationTypeId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAccountViolationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAccountViolationTypes_CommonAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CommonAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommonAccountViolationTypes_CommonViolationTypes_ViolationTypeId",
                        column: x => x.ViolationTypeId,
                        principalTable: "CommonViolationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    ExpirationUtc = table.Column<DateTime>(nullable: true),
                    Permissions = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccounts_CommonAccounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "CommonAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccounts_CityId",
                table: "CommonAccounts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccounts_Number",
                table: "CommonAccounts",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccounts_OwnerUserId",
                table: "CommonAccounts",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccounts_PartitionId",
                table: "CommonAccounts",
                column: "PartitionId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccountViolationTypes_AccountId",
                table: "CommonAccountViolationTypes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccountViolationTypes_ViolationTypeId",
                table: "CommonAccountViolationTypes",
                column: "ViolationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_AccountId",
                table: "UserAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_UserId_AccountId",
                table: "UserAccounts",
                columns: new[] { "UserId", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonUserDevices_UserId",
                table: "CommonUserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonViolations_CategoryId_Name",
                table: "CommonViolations",
                columns: new[] { "CategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonViolationCategories_TypeId_Name",
                table: "CommonViolationCategories",
                columns: new[] { "TypeId", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonAccountSettings");

            migrationBuilder.DropTable(
                name: "CommonAccountViolationTypes");

            migrationBuilder.DropTable(
                name: "UserAccounts");

            migrationBuilder.DropTable(
                name: "CommonUserDevices");

            migrationBuilder.DropTable(
                name: "CommonViolations");

            migrationBuilder.DropTable(
                name: "CommonAccounts");

            migrationBuilder.DropTable(
                name: "CommonViolationCategories");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Partitions");

            migrationBuilder.DropTable(
                name: "CommonViolationTypes");
        }
    }
}
