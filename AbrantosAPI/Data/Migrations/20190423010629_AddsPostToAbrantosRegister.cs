using Microsoft.EntityFrameworkCore.Migrations;

namespace AbrantosAPI.Data.Migrations
{
    public partial class AddsPostToAbrantosRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Post",
                table: "DailyRegister",
                maxLength: 140,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Post",
                table: "DailyRegister");
        }
    }
}
