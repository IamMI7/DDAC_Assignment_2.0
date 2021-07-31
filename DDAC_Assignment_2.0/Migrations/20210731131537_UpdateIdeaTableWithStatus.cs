using Microsoft.EntityFrameworkCore.Migrations;

namespace DDAC_Assignment_2._0.Migrations
{
    public partial class UpdateIdeaTableWithStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Ideas");
        }
    }
}
