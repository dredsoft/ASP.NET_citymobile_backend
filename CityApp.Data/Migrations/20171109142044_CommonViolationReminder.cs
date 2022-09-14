using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations
{
    public partial class CommonViolationReminder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReminderMessage",
                table: "CommonViolations",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReminderMinutes",
                table: "CommonViolations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderMessage",
                table: "CommonViolations");

            migrationBuilder.DropColumn(
                name: "ReminderMinutes",
                table: "CommonViolations");
        }
    }
}
