using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class boundaryOrderFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
