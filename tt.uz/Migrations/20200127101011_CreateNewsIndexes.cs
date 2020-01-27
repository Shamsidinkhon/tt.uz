using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class CreateNewsIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_News_OwnerId",
                table: "News",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_News_OwnerId",
                table: "News");
        }
    }
}
