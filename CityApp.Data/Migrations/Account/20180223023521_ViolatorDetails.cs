using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class ViolatorDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ViolatorAddress1",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorAddress2",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorCity",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorCountry",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorFirstName",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorLastName",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorState",
                table: "Citations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ViolatorZip",
                table: "Citations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViolatorAddress1",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorAddress2",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorCity",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorCountry",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorFirstName",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorLastName",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorState",
                table: "Citations");

            migrationBuilder.DropColumn(
                name: "ViolatorZip",
                table: "Citations");
        }
    }
}
