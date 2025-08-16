using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddClientMac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mac",
                table: "Clients",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mac",
                table: "Clients");
        }
    }
}
