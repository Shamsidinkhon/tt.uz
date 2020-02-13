using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class AddedSomeIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VendorReviews_TargetUserId",
                table: "VendorReviews",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsAttribute_NewsId",
                table: "NewsAttribute",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_NewsId",
                table: "Images",
                column: "NewsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorReviews_TargetUserId",
                table: "VendorReviews");

            migrationBuilder.DropIndex(
                name: "IX_NewsAttribute_NewsId",
                table: "NewsAttribute");

            migrationBuilder.DropIndex(
                name: "IX_Images_NewsId",
                table: "Images");
        }
    }
}
