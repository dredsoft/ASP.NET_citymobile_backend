using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class CitationAuditLogEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "CitationAuditLogs");

            migrationBuilder.AddColumn<int>(
                name: "Event",
                table: "CitationAuditLogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Event",
                table: "CitationAuditLogs");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "CitationAuditLogs",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
