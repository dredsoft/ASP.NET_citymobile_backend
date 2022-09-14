using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    FirstName = table.Column<string>(maxLength: 50, nullable: true),
                    LastName = table.Column<string>(maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViolationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CommonViolationTypeId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    CustomDescription = table.Column<string>(maxLength: 500, nullable: true),
                    CustomName = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    Disabled = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationTypes_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ContentLength = table.Column<long>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    FileName = table.Column<string>(maxLength: 255, nullable: false),
                    Key = table.Column<string>(maxLength: 255, nullable: false),
                    MimeType = table.Column<string>(maxLength: 100, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachments_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Counters",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    NextValue = table.Column<long>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Counters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Counters_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Counters_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Counters_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Actions = table.Column<int>(nullable: false),
                    Address1 = table.Column<string>(maxLength: 250, nullable: true),
                    Address2 = table.Column<string>(maxLength: 250, nullable: true),
                    City = table.Column<string>(maxLength: 200, nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    MobilePhone = table.Column<string>(maxLength: 15, nullable: true),
                    Name = table.Column<string>(maxLength: 150, nullable: true),
                    OfficePhone = table.Column<string>(maxLength: 15, nullable: true),
                    State = table.Column<string>(maxLength: 50, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vendors_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendors_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vendors_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViolationCategorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CommonCategoryId = table.Column<Guid>(nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    CustomDescription = table.Column<string>(maxLength: 500, nullable: true),
                    CustomName = table.Column<string>(maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_ViolationCategorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationCategorys_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationCategorys_ViolationTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ViolationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountUserVendors",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountUserId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountUserVendors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountUserVendors_AccountUsers_AccountUserId",
                        column: x => x.AccountUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountUserVendors_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Violations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Actions = table.Column<int>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: true),
                    Code = table.Column<string>(nullable: true),
                    CommonViolationId = table.Column<Guid>(nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    CustomActions = table.Column<int>(nullable: false),
                    CustomDescription = table.Column<string>(maxLength: 500, nullable: true),
                    CustomHelpUrl = table.Column<string>(maxLength: 250, nullable: true),
                    CustomName = table.Column<string>(maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_Violations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Violations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Violations_ViolationCategorys_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ViolationCategorys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Citations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AssignedToId = table.Column<Guid>(nullable: true),
                    CitationNumber = table.Column<long>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    LicensePlate = table.Column<string>(maxLength: 25, nullable: true),
                    LocationDescription = table.Column<string>(maxLength: 255, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    VehicleColor = table.Column<string>(maxLength: 50, nullable: true),
                    VehicleMake = table.Column<string>(maxLength: 50, nullable: true),
                    VehicleModel = table.Column<string>(maxLength: 50, nullable: true),
                    VehicleType = table.Column<string>(maxLength: 50, nullable: true),
                    ViolationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Citations_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citations_AccountUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citations_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Citations_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Citations_Violations_ViolationId",
                        column: x => x.ViolationId,
                        principalTable: "Violations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CitationAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AttachmentId = table.Column<Guid>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitationAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitationAttachments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationAttachments_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationAttachments_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationAttachments_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationAttachments_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CitationComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitationComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitationComment_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationComment_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationComment_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationComment_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountUsers_Email",
                table: "AccountUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountUserVendors_AccountUserId",
                table: "AccountUserVendors",
                column: "AccountUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountUserVendors_VendorId",
                table: "AccountUserVendors",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AccountId",
                table: "Attachments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CreateUserId",
                table: "Attachments",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_UpdateUserId",
                table: "Attachments",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_AccountId",
                table: "Citations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_AssignedToId",
                table: "Citations",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_CreateUserId",
                table: "Citations",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_Status",
                table: "Citations",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_UpdateUserId",
                table: "Citations",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Citations_ViolationId",
                table: "Citations",
                column: "ViolationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAttachments_AccountId",
                table: "CitationAttachments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAttachments_AttachmentId",
                table: "CitationAttachments",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAttachments_CitationId",
                table: "CitationAttachments",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAttachments_CreateUserId",
                table: "CitationAttachments",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAttachments_UpdateUserId",
                table: "CitationAttachments",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationComment_AccountId",
                table: "CitationComment",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationComment_CitationId",
                table: "CitationComment",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationComment_CreateUserId",
                table: "CitationComment",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationComment_UpdateUserId",
                table: "CitationComment",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Counters_AccountId",
                table: "Counters",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Counters_CreateUserId",
                table: "Counters",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Counters_UpdateUserId",
                table: "Counters",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Counters_Name_AccountId",
                table: "Counters",
                columns: new[] { "Name", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_AccountId",
                table: "Vendors",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CreateUserId",
                table: "Vendors",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_UpdateUserId",
                table: "Vendors",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_Name_AccountId",
                table: "Vendors",
                columns: new[] { "Name", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Violations_AccountId",
                table: "Violations",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Violations_Code",
                table: "Violations",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Violations_CategoryId_Name",
                table: "Violations",
                columns: new[] { "CategoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViolationCategorys_AccountId",
                table: "ViolationCategorys",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationCategorys_TypeId_Name",
                table: "ViolationCategorys",
                columns: new[] { "TypeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViolationTypes_AccountId",
                table: "ViolationTypes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
               name: "IX_Citations_CitationNumber_AccountId",
               table: "Citations",
               columns: new[] { "CitationNumber", "AccountId" },
               unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountUserVendors");

            migrationBuilder.DropTable(
                name: "CitationAttachments");

            migrationBuilder.DropTable(
                name: "CitationComment");

            migrationBuilder.DropTable(
                name: "Counters");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Citations");

            migrationBuilder.DropTable(
                name: "AccountUsers");

            migrationBuilder.DropTable(
                name: "Violations");

            migrationBuilder.DropTable(
                name: "ViolationCategorys");

            migrationBuilder.DropTable(
                name: "ViolationTypes");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
