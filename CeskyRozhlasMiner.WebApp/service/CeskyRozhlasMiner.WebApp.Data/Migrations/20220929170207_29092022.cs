using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Microsoft.DSX.ProjectTemplate.Data.Migrations
{
    public partial class _29092022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
