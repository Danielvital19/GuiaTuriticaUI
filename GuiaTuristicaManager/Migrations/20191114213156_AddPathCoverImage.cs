using Microsoft.EntityFrameworkCore.Migrations;

namespace GuiaTuristicaManager.Migrations
{
    public partial class AddPathCoverImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PathCover",
                table: "Zones",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PathCover",
                table: "Zones");
        }
    }
}
