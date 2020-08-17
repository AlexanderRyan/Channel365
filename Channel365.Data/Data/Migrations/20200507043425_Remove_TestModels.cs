using Microsoft.EntityFrameworkCore.Migrations;

namespace Channel365.Web.Data.Migrations
{
    public partial class Remove_TestModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestModels");

        //    migrationBuilder.CreateTable(
        //        name: "UserFollow",
        //        columns: table => new
        //        {
        //            TargetId = table.Column<string>(nullable: false),
        //            ObserverId = table.Column<string>(nullable: false),
        //            Follow = table.Column<bool>(nullable: false)
        //        },
        //        constraints: table =>
        //        {
        //            table.PrimaryKey("PK_UserFollow", x => new { x.ObserverId, x.TargetId });
        //            table.ForeignKey(
        //                name: "FK_UserFollow_AspNetUsers_ObserverId",
        //                column: x => x.ObserverId,
        //                principalTable: "AspNetUsers",
        //                principalColumn: "Id",
        //                onDelete: ReferentialAction.Restrict);
        //            table.ForeignKey(
        //                name: "FK_UserFollow_AspNetUsers_TargetId",
        //                column: x => x.TargetId,
        //                principalTable: "AspNetUsers",
        //                principalColumn: "Id",
        //                onDelete: ReferentialAction.Restrict);
        //        });

        //    migrationBuilder.CreateIndex(
        //        name: "IX_UserFollow_TargetId",
        //        table: "UserFollow",
        //        column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFollow");

            migrationBuilder.CreateTable(
                name: "TestModels",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UrlSlug = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestModels", x => x.Id);
                });
        }
    }
}
