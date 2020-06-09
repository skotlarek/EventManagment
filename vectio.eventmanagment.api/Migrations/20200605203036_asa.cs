using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class asa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUser_Events_EventId",
                table: "EventUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser");

            migrationBuilder.RenameTable(
                name: "EventUser",
                newName: "EventUsers");

            migrationBuilder.RenameIndex(
                name: "IX_EventUser_EventId",
                table: "EventUsers",
                newName: "IX_EventUsers_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers");

            migrationBuilder.RenameTable(
                name: "EventUsers",
                newName: "EventUser");

            migrationBuilder.RenameIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUser",
                newName: "IX_EventUser_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUser_Events_EventId",
                table: "EventUser",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
