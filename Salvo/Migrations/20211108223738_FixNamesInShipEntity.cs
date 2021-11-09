using Microsoft.EntityFrameworkCore.Migrations;

namespace Salvo.Migrations
{
    public partial class FixNamesInShipEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cell",
                table: "ShipLocations",
                newName: "Location");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Ships",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Ships");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "ShipLocations",
                newName: "cell");
        }
    }
}
