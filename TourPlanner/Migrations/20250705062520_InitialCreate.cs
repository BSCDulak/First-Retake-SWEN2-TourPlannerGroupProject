using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWEN2_TourPlannerGroupProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "FromLatitude",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FromLongitude",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToLatitude",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ToLongitude",
                table: "Tours",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "TourLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromLatitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "FromLongitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ToLatitude",
                table: "Tours");

            migrationBuilder.DropColumn(
                name: "ToLongitude",
                table: "Tours");

            migrationBuilder.AlterColumn<int>(
                name: "TourId",
                table: "TourLogs",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
