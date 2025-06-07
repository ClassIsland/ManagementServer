using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAllowChangePassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowChangePassword",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowChangePassword",
                table: "AspNetUsers");
        }
    }
}
