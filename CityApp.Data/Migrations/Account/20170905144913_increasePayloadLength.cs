using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class increasePayloadLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReceiptPayload",
                table: "CitationReceipts",
                maxLength: 2147483647,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 500,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReceiptPayload",
                table: "CitationReceipts",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 2147483647,
                oldNullable: true);
        }
    }
}
