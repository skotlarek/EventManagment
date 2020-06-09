using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class _15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "LimitedPlaces",
                table: "Events",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "0");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "LimitedPlaces",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValueSql: "0",
                oldClrType: typeof(bool));
        }
    }
}
