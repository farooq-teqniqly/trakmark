using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakmark.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisteredUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredUsers",
                columns: table => new
                {
                    RegisteredUserId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredUsers", x => x.RegisteredUserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegisteredUsers_AccountId",
                table: "RegisteredUsers",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisteredUsers");
        }
    }
}
