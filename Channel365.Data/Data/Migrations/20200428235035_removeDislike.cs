using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class removeDislike : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dislike",
                table: "LikesAndDislikes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Dislike",
                table: "LikesAndDislikes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
