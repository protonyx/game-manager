using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Players",
                type: "TEXT",
                maxLength: 7,
                nullable: false,
                defaultValue: "#D32F2F");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Players");
        }
    }
}
