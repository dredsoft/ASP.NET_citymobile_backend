using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class CitationAuditLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CitationAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitationAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitationAuditLogs_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationAuditLogs_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationAuditLogs_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationAuditLogs_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitationAuditLogs_AccountId",
                table: "CitationAuditLogs",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAuditLogs_CitationId",
                table: "CitationAuditLogs",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAuditLogs_CreateUserId",
                table: "CitationAuditLogs",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationAuditLogs_UpdateUserId",
                table: "CitationAuditLogs",
                column: "UpdateUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitationAuditLogs");
        }
    }
}
