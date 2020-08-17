using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class ResetDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UrlSlug",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserImageUrl",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(nullable: false),
                    PlaylistName = table.Column<string>(nullable: true),
                    PlaylistDesc = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true),
                    PlaylistVisibility = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.PlaylistId);
                    table.ForeignKey(
                        name: "FK_Playlists_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestModels",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VideoLanguages",
                columns: table => new
                {
                    VideoLangId = table.Column<Guid>(nullable: false),
                    LanguageName = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLanguages", x => x.VideoLangId);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    VideoId = table.Column<Guid>(nullable: false),
                    VideoTitle = table.Column<string>(nullable: true),
                    VideoDescription = table.Column<string>(nullable: true),
                    MadeForKids = table.Column<int>(nullable: true),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true),
                    VideoPath = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    VideoLanguageModelVideoLangId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.VideoId);
                    table.ForeignKey(
                        name: "FK_Videos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Videos_VideoLanguages_VideoLanguageModelVideoLangId",
                        column: x => x.VideoLanguageModelVideoLangId,
                        principalTable: "VideoLanguages",
                        principalColumn: "VideoLangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentVideos",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(nullable: false),
                    CommentBody = table.Column<string>(nullable: true),
                    UrlSlug = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    ApplicationUserId = table.Column<string>(nullable: true),
                    VideoModelVideoId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentVideos", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_CommentVideos_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentVideos_Videos_VideoModelVideoId",
                        column: x => x.VideoModelVideoId,
                        principalTable: "Videos",
                        principalColumn: "VideoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VideosInPlaylist",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(nullable: false),
                    VideoId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosInPlaylist", x => new { x.PlaylistId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_VideosInPlaylist_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "PlaylistId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosInPlaylist_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "VideoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVideos_ApplicationUserId",
                table: "CommentVideos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentVideos_VideoModelVideoId",
                table: "CommentVideos",
                column: "VideoModelVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ApplicationUserId",
                table: "Videos",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoLanguageModelVideoLangId",
                table: "Videos",
                column: "VideoLanguageModelVideoLangId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosInPlaylist_VideoId",
                table: "VideosInPlaylist",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CommentVideos");

            migrationBuilder.DropTable(
                name: "TestModels");

            migrationBuilder.DropTable(
                name: "VideosInPlaylist");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "VideoLanguages");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UrlSlug",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserImageUrl",
                table: "AspNetUsers");
        }
    }
}
