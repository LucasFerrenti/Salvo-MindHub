using Microsoft.EntityFrameworkCore.Migrations;

namespace Salvo.Migrations
{
    public partial class UpdateRelationsGP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_GamePlayer_GamePlayerId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_GamePlayer_GamePlayerId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_GamePlayerId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Games_GamePlayerId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GamePlayer",
                table: "GamePlayer");

            migrationBuilder.DropColumn(
                name: "GamePlayerId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GamePlayerId",
                table: "Games");

            migrationBuilder.RenameTable(
                name: "GamePlayer",
                newName: "GamePlayers");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GamePlayers",
                table: "GamePlayers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_GameID",
                table: "GamePlayers",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlayers_PlayerID",
                table: "GamePlayers",
                column: "PlayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_Games_GameID",
                table: "GamePlayers",
                column: "GameID",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GamePlayers_Players_PlayerID",
                table: "GamePlayers",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_Games_GameID",
                table: "GamePlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_GamePlayers_Players_PlayerID",
                table: "GamePlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GamePlayers",
                table: "GamePlayers");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayers_GameID",
                table: "GamePlayers");

            migrationBuilder.DropIndex(
                name: "IX_GamePlayers_PlayerID",
                table: "GamePlayers");

            migrationBuilder.RenameTable(
                name: "GamePlayers",
                newName: "GamePlayer");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_GamePlayer",
                table: "GamePlayer",
                column: "Id");

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
    }
}
