using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SWEN2_TourPlannerGroupProject.Migrations
{
    /// <inheritdoc />
    public partial class BackupIDForImportExport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BackupId",
                table: "Tours",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackupId",
                table: "Tours");
        }
    }
}
