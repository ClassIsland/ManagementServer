using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicyGrouping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "Policies",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_GroupId",
                table: "Policies",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_ProfileGroups_GroupId",
                table: "Policies",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_ProfileGroups_GroupId",
                table: "Policies");

            migrationBuilder.DropIndex(
                name: "IX_Policies_GroupId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Policies");
        }
    }
}
