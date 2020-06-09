using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class NumberSeats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberSeats",
                table: "Events",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberSeats",
                table: "Events");
        }
    }
}
