using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations
{
    public partial class commonAccountTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CommonAccountId",
                table: "CommonAccountViolationTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonAccountViolationTypes_CommonAccountId",
                table: "CommonAccountViolationTypes",
                column: "CommonAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAccountViolationTypes_CommonAccounts_CommonAccountId",
                table: "CommonAccountViolationTypes",
                column: "CommonAccountId",
                principalTable: "CommonAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAccountViolationTypes_CommonAccounts_CommonAccountId",
                table: "CommonAccountViolationTypes");

            migrationBuilder.DropIndex(
                name: "IX_CommonAccountViolationTypes_CommonAccountId",
                table: "CommonAccountViolationTypes");

            migrationBuilder.DropColumn(
                name: "CommonAccountId",
                table: "CommonAccountViolationTypes");
        }
    }
}
