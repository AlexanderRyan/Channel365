using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class removed_vidlang : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_VideoLanguages_VideoLanguageModelVideoLangId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "VideoLanguages");

            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoLanguageModelVideoLangId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "MadeForKids",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoLanguageModelVideoLangId",
                table: "Videos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MadeForKids",
                table: "Videos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VideoLanguageModelVideoLangId",
                table: "Videos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VideoLanguages",
                columns: table => new
                {
                    VideoLangId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlSlug = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLanguages", x => x.VideoLangId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoLanguageModelVideoLangId",
                table: "Videos",
                column: "VideoLanguageModelVideoLangId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_VideoLanguages_VideoLanguageModelVideoLangId",
                table: "Videos",
                column: "VideoLanguageModelVideoLangId",
                principalTable: "VideoLanguages",
                principalColumn: "VideoLangId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
