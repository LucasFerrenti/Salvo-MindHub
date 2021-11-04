using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Salvo.Migrations
{
    public partial class AddGamePlayerEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GamePlayerId",
                table: "Players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "GamePlayerId",
                table: "Games",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GamePlayer",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameID = table.Column<long>(type: "bigint", nullable: false),
                    PlayerID = table.Column<long>(type: "bigint", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlayer", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GamePlayerId",
                table: "Players",
                column: "GamePlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GamePlayerId",
                table: "Games",
                column: "GamePlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Games_GamePlayer_GamePlayerId",
                table: "Games",
                column: "GamePlayerId",
                principalTable: "GamePlayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_GamePlayer_GamePlayerId",
                table: "Players",
                column: "GamePlayerId",
                principalTable: "GamePlayer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GamePlayer_GamePlayerId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_GamePlayer_GamePlayerId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "GamePlayer");

            migrationBuilder.DropIndex(
                name: "IX_Players_GamePlayerId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Games_GamePlayerId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GamePlayerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GamePlayerId",
                table: "Games");
        }
    }
}
