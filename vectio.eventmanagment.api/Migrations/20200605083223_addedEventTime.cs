using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class addedEventTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DurationTime",
                table: "Events",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "EventTime",
                table: "Events",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventTime",
                table: "Events");

            migrationBuilder.AlterColumn<decimal>(
                name: "DurationTime",
                table: "Events",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
