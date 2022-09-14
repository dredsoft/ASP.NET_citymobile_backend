using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class CitationAddressFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Citations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Citations");
        }
    }
}
