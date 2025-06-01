using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class DropFKs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectUpdates_Clients_TargetClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ObjectUpdates_TargetClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.DropColumn(
                name: "TargetClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.AddColumn<Guid>(
                name: "ClientCuid",
                table: "ObjectUpdates",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectUpdates_ClientCuid",
                table: "ObjectUpdates",
                column: "ClientCuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectUpdates_Clients_ClientCuid",
                table: "ObjectUpdates",
                column: "ClientCuid",
                principalTable: "Clients",
                principalColumn: "Cuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectUpdates_Clients_ClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.DropIndex(
                name: "IX_ObjectUpdates_ClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.DropColumn(
                name: "ClientCuid",
                table: "ObjectUpdates");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetClientCuid",
                table: "ObjectUpdates",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectUpdates_TargetClientCuid",
                table: "ObjectUpdates",
                column: "TargetClientCuid");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectUpdates_Clients_TargetClientCuid",
                table: "ObjectUpdates",
                column: "TargetClientCuid",
                principalTable: "Clients",
                principalColumn: "Cuid",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
