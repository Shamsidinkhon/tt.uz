using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class AddedIndexesEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VendorFavourite_TargetUserId",
                table: "VendorFavourite",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavourites_NewsId",
                table: "UserFavourites",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsAttribute_AttributeId",
                table: "NewsAttribute",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogin_UserId",
                table: "ExternalLogin",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VendorFavourite_TargetUserId",
                table: "VendorFavourite");

            migrationBuilder.DropIndex(
                name: "IX_UserProfile_UserId",
                table: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_UserFavourites_NewsId",
                table: "UserFavourites");

            migrationBuilder.DropIndex(
                name: "IX_NewsAttribute_AttributeId",
                table: "NewsAttribute");

            migrationBuilder.DropIndex(
                name: "IX_ExternalLogin_UserId",
                table: "ExternalLogin");
        }
    }
}
