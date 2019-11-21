using Microsoft.EntityFrameworkCore.Migrations;

namespace GuiaTuristicaManager.Migrations
{
    public partial class RemoveFieldsZone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBuild",
                table: "Zones");

            migrationBuilder.DropColumn(
                name: "PathDatabase",
                table: "Zones");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBuild",
                table: "Zones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PathDatabase",
                table: "Zones",
                nullable: true);
        }
    }
}
