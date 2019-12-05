using Microsoft.EntityFrameworkCore.Migrations;

namespace GuiaTuristicaManager.Migrations
{
    public partial class AddPathWtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathWtc",
                table: "Zones",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathWtc",
                table: "Zones");
        }
    }
}
