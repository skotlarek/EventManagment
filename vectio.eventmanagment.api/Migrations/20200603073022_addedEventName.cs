using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class addedEventName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventName",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventName",
                table: "Events");
        }
    }
}
