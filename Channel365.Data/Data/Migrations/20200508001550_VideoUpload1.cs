using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class VideoUpload1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Videos_VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Videos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserFollow",
                columns: table => new
                {
                    TargetId = table.Column<string>(nullable: false),
                    ObserverId = table.Column<string>(nullable: false),
                    Follow = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollow", x => new { x.ObserverId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_UserFollow_AspNetUsers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollow_AspNetUsers_TargetId",
                        column: x => x.TargetId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ApplicationUserId",
                table: "Videos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollow_TargetId",
                table: "UserFollow",
                column: "TargetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers_ApplicationUserId",
                table: "Videos",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers_ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "UserFollow");

            migrationBuilder.DropIndex(
                name: "IX_Videos_ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Videos");

            migrationBuilder.AddColumn<Guid>(
                name: "VideoModelVideoId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_VideoModelVideoId",
                table: "AspNetUsers",
                column: "VideoModelVideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Videos_VideoModelVideoId",
                table: "AspNetUsers",
                column: "VideoModelVideoId",
                principalTable: "Videos",
                principalColumn: "VideoId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
