using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class UpdateCategoryEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Icon",
                table: "Categories",
                newName: "WebIcon");

            migrationBuilder.AddColumn<string>(
                name: "MobileIcon",
                table: "Categories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileIcon",
                table: "Categories");

            migrationBuilder.RenameColumn(
                name: "WebIcon",
                table: "Categories",
                newName: "Icon");
        }
    }
}
