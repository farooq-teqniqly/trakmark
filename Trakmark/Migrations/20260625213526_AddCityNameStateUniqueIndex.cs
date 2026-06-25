using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trakmark.Migrations
{
    /// <inheritdoc />
    public partial class AddCityNameStateUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name_State",
                table: "Cities",
                columns: new[] { "Name", "State" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cities_Name_State",
                table: "Cities");
        }
    }
}
