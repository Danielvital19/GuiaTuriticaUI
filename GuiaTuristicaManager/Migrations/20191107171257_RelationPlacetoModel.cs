using Microsoft.EntityFrameworkCore.Migrations;

namespace GuiaTuristicaManager.Migrations
{
    public partial class RelationPlacetoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_PlaceId",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_PlaceId",
                table: "Models",
                column: "PlaceId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Models_PlaceId",
                table: "Models");

            migrationBuilder.CreateIndex(
                name: "IX_Models_PlaceId",
                table: "Models",
                column: "PlaceId");
        }
    }
}
