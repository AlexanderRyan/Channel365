using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikesAndDislikes",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    VideoId = table.Column<Guid>(nullable: false),
                    Like = table.Column<bool>(nullable: false),
                    Dislike = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikesAndDislikes", x => new { x.UserId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_LikesAndDislikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikesAndDislikes_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "VideoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikesAndDislikes_VideoId",
                table: "LikesAndDislikes",
                column: "VideoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikesAndDislikes");
        }
    }
}
