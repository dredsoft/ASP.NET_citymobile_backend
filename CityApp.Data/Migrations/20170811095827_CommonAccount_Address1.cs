using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations
{
    public partial class CommonAccount_Address1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "CommonAccounts",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "CommonAccounts",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityName",
                table: "CommonAccounts",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "CommonAccounts",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CommonAccounts",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "CommonAccounts",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "CommonAccounts");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "CommonAccounts");

            migrationBuilder.DropColumn(
                name: "CityName",
                table: "CommonAccounts");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "CommonAccounts");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CommonAccounts");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "CommonAccounts");
        }
    }
}
