using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class warningTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosedReason",
                table: "Citations",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Agency",
                table: "AccountUsers",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BadgeNumber",
                table: "AccountUsers",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WarningEventRespones",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Payload = table.Column<string>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarningEventRespones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WarningEventRespones_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarningEventRespones_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WarningEventRespones_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WarningEventRespones_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarningEventRespones_AccountId",
                table: "WarningEventRespones",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningEventRespones_CitationId",
                table: "WarningEventRespones",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningEventRespones_CreateUserId",
                table: "WarningEventRespones",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WarningEventRespones_UpdateUserId",
                table: "WarningEventRespones",
                column: "UpdateUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarningEventRespones");

            migrationBuilder.DropColumn(
                name: "ClosedReason",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "Agency",
                table: "AccountUsers");

            migrationBuilder.DropColumn(
                name: "BadgeNumber",
                table: "AccountUsers");
        }
    }
}
