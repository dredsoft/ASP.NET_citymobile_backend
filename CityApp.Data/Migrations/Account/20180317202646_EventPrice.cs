using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class EventPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "End",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Start",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventViolationPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    EventId1 = table.Column<Guid>(nullable: true),
                    Fee = table.Column<double>(nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    ViolationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventViolationPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_Events_EventId1",
                        column: x => x.EventId1,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventViolationPrices_Violations_ViolationId",
                        column: x => x.ViolationId,
                        principalTable: "Violations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_AccountId",
                table: "EventViolationPrices",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_CreateUserId",
                table: "EventViolationPrices",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_EventId",
                table: "EventViolationPrices",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_EventId1",
                table: "EventViolationPrices",
                column: "EventId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_UpdateUserId",
                table: "EventViolationPrices",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventViolationPrices_ViolationId",
                table: "EventViolationPrices",
                column: "ViolationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventViolationPrices");

            migrationBuilder.DropColumn(
                name: "End",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Start",
                table: "Events");
        }
    }
}
