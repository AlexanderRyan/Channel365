using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class AddAppUserToPlaylist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Playlists",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Playlists");
        }
    }
}
