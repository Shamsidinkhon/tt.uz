using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tt.uz.Migrations
{
    public partial class RegionsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Lang = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    ParentId = table.Column<int>(nullable: false),
                    SoatoId = table.Column<int>(nullable: false),
                    Depth = table.Column<int>(nullable: false),
                    Available = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
