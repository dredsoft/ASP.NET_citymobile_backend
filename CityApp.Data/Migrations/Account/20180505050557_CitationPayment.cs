using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class CitationPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventViolationPrices_Violations_ViolationId1",
                table: "EventViolationPrices");

            migrationBuilder.DropIndex(
                name: "IX_EventViolationPrices_ViolationId1",
                table: "EventViolationPrices");

            migrationBuilder.DropColumn(
                name: "ViolationId1",
                table: "EventViolationPrices");

            migrationBuilder.CreateTable(
                name: "CitationPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ChargeAmount = table.Column<int>(nullable: false),
                    ChargeId = table.Column<string>(nullable: true),
                    CitationFineAmount = table.Column<double>(nullable: false),
                    CitationId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    ProcessingFee = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitationPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CitationPayments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationPayments_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CitationPayments_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CitationPayments_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitationPayments_AccountId",
                table: "CitationPayments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationPayments_CitationId",
                table: "CitationPayments",
                column: "CitationId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationPayments_CreateUserId",
                table: "CitationPayments",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CitationPayments_UpdateUserId",
                table: "CitationPayments",
                column: "UpdateUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitationPayments");

            migrationBuilder.AddColumn<Guid>(
                name: "ViolationId1",
                table: "EventViolationPrices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_ViolationId1",
                table: "EventViolationPrices",
                column: "ViolationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EventViolationPrices_Violations_ViolationId1",
                table: "EventViolationPrices",
                column: "ViolationId1",
                principalTable: "Violations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
