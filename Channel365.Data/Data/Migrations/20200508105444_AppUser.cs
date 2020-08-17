using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class AppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_AspNetUsers_ObserverId",
                table: "UserFollow");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollow_AspNetUsers_TargetId",
                table: "UserFollow");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollow",
                table: "UserFollow");

            migrationBuilder.RenameTable(
                name: "UserFollow",
                newName: "UserFollowings");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollow_TargetId",
                table: "UserFollowings",
                newName: "IX_UserFollowings_TargetId");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Playlists",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollowings",
                table: "UserFollowings",
                columns: new[] { "ObserverId", "TargetId" });

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_AspNetUsers_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_AspNetUsers_ObserverId",
                table: "UserFollowings",
                column: "ObserverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollowings_AspNetUsers_TargetId",
                table: "UserFollowings",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_AspNetUsers_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_AspNetUsers_ObserverId",
                table: "UserFollowings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserFollowings_AspNetUsers_TargetId",
                table: "UserFollowings");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFollowings",
                table: "UserFollowings");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Playlists");

            migrationBuilder.RenameTable(
                name: "UserFollowings",
                newName: "UserFollow");

            migrationBuilder.RenameIndex(
                name: "IX_UserFollowings_TargetId",
                table: "UserFollow",
                newName: "IX_UserFollow_TargetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFollow",
                table: "UserFollow",
                columns: new[] { "ObserverId", "TargetId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_AspNetUsers_ObserverId",
                table: "UserFollow",
                column: "ObserverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserFollow_AspNetUsers_TargetId",
                table: "UserFollow",
                column: "TargetId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
