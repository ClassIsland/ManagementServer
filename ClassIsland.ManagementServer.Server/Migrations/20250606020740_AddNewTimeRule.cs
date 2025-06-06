using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddNewTimeRule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeekCountDivTotal",
                table: "ProfileClassplans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeekCountDivTotal",
                table: "ProfileClassplans");
        }
    }
}
