using Microsoft.EntityFrameworkCore.Migrations;

namespace DDAC_Assignment_2._0.Migrations
{
    public partial class removeCuratorRolefromIdeasTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuratorRole",
                table: "Ideas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CuratorRole",
                table: "Ideas",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }
    }
}
