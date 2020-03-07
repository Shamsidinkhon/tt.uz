using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class TariffIndexAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tariff_NewsId",
                table: "Tariff",
                column: "NewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tariff_NewsId",
                table: "Tariff");
        }
    }
}
