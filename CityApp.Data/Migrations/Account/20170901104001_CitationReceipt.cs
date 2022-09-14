using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class CitationReceipt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CitationReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    ReceiptPayload = table.Column<string>(maxLength: 500, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitationReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitationReceipts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationReceipts_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationReceipts_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationReceipts_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitationReceipts_AccountId",
                table: "CitationReceipts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationReceipts_CitationId",
                table: "CitationReceipts",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationReceipts_CreateUserId",
                table: "CitationReceipts",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationReceipts_UpdateUserId",
                table: "CitationReceipts",
                column: "UpdateUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitationReceipts");
        }
    }
}
