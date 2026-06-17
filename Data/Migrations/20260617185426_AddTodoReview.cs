using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzubiLog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                table: "Todos",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewStatus",
                table: "Todos",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "Offen");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByUserId",
                table: "Todos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Todos_ReviewedByUserId",
                table: "Todos",
                column: "ReviewedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_AspNetUsers_ReviewedByUserId",
                table: "Todos",
                column: "ReviewedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_AspNetUsers_ReviewedByUserId",
                table: "Todos");

            migrationBuilder.DropIndex(
                name: "IX_Todos_ReviewedByUserId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ReviewComment",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ReviewStatus",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "ReviewedByUserId",
                table: "Todos");
        }
    }
}
