using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakmark.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditColumnsToRegisteredUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "RegisteredUsers",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "RegisteredUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RegisteredUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RegisteredUsers");
        }
    }
}
