using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class AddNewVideoAndCommentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentVideos_Videos_VideoModelVideoId",
                table: "CommentVideos");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_AspNetUsers_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers_ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_CommentVideos_VideoModelVideoId",
                table: "CommentVideos");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "VideoModelVideoId",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "CommentVideos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAt",
                table: "CommentVideos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "VideoId",
                table: "CommentVideos",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VideoModelVideoId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentVideos_VideoId",
                table: "CommentVideos",
                column: "VideoId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVideos_Videos_VideoId",
                table: "CommentVideos",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "VideoId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Videos_VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommentVideos_Videos_VideoId",
                table: "CommentVideos");

            migrationBuilder.DropIndex(
                name: "IX_CommentVideos_VideoId",
                table: "CommentVideos");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "PostedAt",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "CommentVideos");

            migrationBuilder.DropColumn(
                name: "VideoModelVideoId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Videos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Playlists",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "CommentVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                table: "CommentVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VideoModelVideoId",
                table: "CommentVideos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ApplicationUserId",
                table: "Videos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVideos_VideoModelVideoId",
                table: "CommentVideos",
                column: "VideoModelVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommentVideos_Videos_VideoModelVideoId",
                table: "CommentVideos",
                column: "VideoModelVideoId",
                principalTable: "Videos",
                principalColumn: "VideoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_AspNetUsers_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers_ApplicationUserId",
                table: "Videos",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
