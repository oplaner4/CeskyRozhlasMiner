using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microsoft.DSX.ProjectTemplate.Data.Migrations
{
    public partial class _29092022_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
