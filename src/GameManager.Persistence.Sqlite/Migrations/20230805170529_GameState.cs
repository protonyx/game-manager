using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameManager.Server.Migrations
{
    /// <inheritdoc />
    public partial class GameState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastTurnStartTime",
                table: "Games",
                newName: "CurrentTurnStartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "Games",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Games",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "CurrentTurnStartTime",
                table: "Games",
                newName: "LastTurnStartTime");
        }
    }
}
