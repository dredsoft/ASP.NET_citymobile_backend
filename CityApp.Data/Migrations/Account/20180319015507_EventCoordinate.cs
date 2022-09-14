using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class EventCoordinate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventBoundaryCoordinates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<Guid>(nullable: false),
                    EventId1 = table.Column<Guid>(nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(18,12)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,12)", nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventBoundaryCoordinates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventBoundaryCoordinates_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventBoundaryCoordinates_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventBoundaryCoordinates_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventBoundaryCoordinates_Events_EventId1",
                        column: x => x.EventId1,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventBoundaryCoordinates_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventBoundaryCoordinates_AccountId",
                table: "EventBoundaryCoordinates",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_EventBoundaryCoordinates_CreateUserId",
                table: "EventBoundaryCoordinates",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventBoundaryCoordinates_EventId",
                table: "EventBoundaryCoordinates",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventBoundaryCoordinates_EventId1",
                table: "EventBoundaryCoordinates",
                column: "EventId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventBoundaryCoordinates_UpdateUserId",
                table: "EventBoundaryCoordinates",
                column: "UpdateUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventBoundaryCoordinates");
        }
    }
}
