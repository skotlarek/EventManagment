using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vectio.eventmanagement.api.Migrations
{
    public partial class sad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "EventUser",
                nullable: false,
                defaultValueSql: "newsequentialid()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "EventUser",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldDefaultValueSql: "newsequentialid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUser",
                table: "EventUser",
                column: "Id");
        }
    }
}
