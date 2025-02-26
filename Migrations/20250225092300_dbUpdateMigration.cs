using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorApi.Migrations
{
    /// <inheritdoc />
    public partial class dbUpdateMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TempPasswordExpiry",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryPassword",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "TempPasswordExpiry",
                table: "Applications",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TemporaryEmail",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TemporaryPassword",
                table: "Applications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempPasswordExpiry",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TemporaryPassword",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TempPasswordExpiry",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "TemporaryEmail",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "TemporaryPassword",
                table: "Applications");
        }
    }
}
