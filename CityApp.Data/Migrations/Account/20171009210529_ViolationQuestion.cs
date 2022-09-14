using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CityApp.Data.Migrations.Account
{
    public partial class ViolationQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Violations_Code",
                table: "Violations");

            migrationBuilder.CreateTable(
                name: "ViolationQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Choices = table.Column<string>(maxLength: 500, nullable: true),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    IsRequired = table.Column<bool>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Question = table.Column<string>(maxLength: 500, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    ViolationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationQuestions_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationQuestions_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationQuestions_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViolationQuestions_Violations_ViolationId",
                        column: x => x.ViolationId,
                        principalTable: "Violations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViolationQuestionAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Answer = table.Column<string>(maxLength: 500, nullable: true),
                    CitationId = table.Column<Guid>(nullable: false),
                    CreateUserId = table.Column<Guid>(nullable: false),
                    CreateUtc = table.Column<DateTime>(nullable: false),
                    Question = table.Column<string>(maxLength: 500, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UpdateUserId = table.Column<Guid>(nullable: false),
                    UpdateUtc = table.Column<DateTime>(nullable: false),
                    ViolationQuestionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationQuestionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationQuestionAnswers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationQuestionAnswers_Citations_CitationId",
                        column: x => x.CitationId,
                        principalTable: "Citations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViolationQuestionAnswers_AccountUsers_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationQuestionAnswers_AccountUsers_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "AccountUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ViolationQuestionAnswers_ViolationQuestions_ViolationQuestionId",
                        column: x => x.ViolationQuestionId,
                        principalTable: "ViolationQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Violations_Code_AccountId",
                table: "Violations",
                columns: new[] { "Code", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestions_AccountId",
                table: "ViolationQuestions",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestions_CreateUserId",
                table: "ViolationQuestions",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestions_UpdateUserId",
                table: "ViolationQuestions",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestions_ViolationId",
                table: "ViolationQuestions",
                column: "ViolationId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestions_Question_AccountId",
                table: "ViolationQuestions",
                columns: new[] { "Question", "AccountId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestionAnswers_AccountId",
                table: "ViolationQuestionAnswers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestionAnswers_CreateUserId",
                table: "ViolationQuestionAnswers",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestionAnswers_UpdateUserId",
                table: "ViolationQuestionAnswers",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestionAnswers_ViolationQuestionId",
                table: "ViolationQuestionAnswers",
                column: "ViolationQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationQuestionAnswers_CitationId_AccountId",
                table: "ViolationQuestionAnswers",
                columns: new[] { "CitationId", "AccountId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViolationQuestionAnswers");

            migrationBuilder.DropTable(
                name: "ViolationQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Violations_Code_AccountId",
                table: "Violations");

            migrationBuilder.CreateIndex(
                name: "IX_Violations_Code",
                table: "Violations",
                column: "Code",
                unique: true);
        }
    }
}
