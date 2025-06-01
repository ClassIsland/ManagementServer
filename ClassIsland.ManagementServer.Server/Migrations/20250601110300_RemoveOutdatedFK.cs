using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOutdatedFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectsAssignees_ClientGroups_TargetGroupId",
                table: "ObjectsAssignees");

            migrationBuilder.DropForeignKey(
                name: "FK_ObjectsAssignees_Clients_TargetClientCuid",
                table: "ObjectsAssignees");

            migrationBuilder.DropIndex(
                name: "IX_ObjectsAssignees_TargetClientCuid",
                table: "ObjectsAssignees");

            migrationBuilder.DropIndex(
                name: "IX_ObjectsAssignees_TargetGroupId",
                table: "ObjectsAssignees");

            migrationBuilder.AddColumn<long>(
                name: "ClientGroupId",
                table: "ObjectsAssignees",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsAssignees_ClientGroupId",
                table: "ObjectsAssignees",
                column: "ClientGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectsAssignees_ClientGroups_ClientGroupId",
                table: "ObjectsAssignees",
                column: "ClientGroupId",
                principalTable: "ClientGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ObjectsAssignees_ClientGroups_ClientGroupId",
                table: "ObjectsAssignees");

            migrationBuilder.DropIndex(
                name: "IX_ObjectsAssignees_ClientGroupId",
                table: "ObjectsAssignees");

            migrationBuilder.DropColumn(
                name: "ClientGroupId",
                table: "ObjectsAssignees");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsAssignees_TargetClientCuid",
                table: "ObjectsAssignees",
                column: "TargetClientCuid");

            migrationBuilder.CreateIndex(
                name: "IX_ObjectsAssignees_TargetGroupId",
                table: "ObjectsAssignees",
                column: "TargetGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectsAssignees_ClientGroups_TargetGroupId",
                table: "ObjectsAssignees",
                column: "TargetGroupId",
                principalTable: "ClientGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ObjectsAssignees_Clients_TargetClientCuid",
                table: "ObjectsAssignees",
                column: "TargetClientCuid",
                principalTable: "Clients",
                principalColumn: "Cuid");
        }
    }
}
