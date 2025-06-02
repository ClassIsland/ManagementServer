using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class RefactorClientManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_ClientGroups_GroupId",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_GroupId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Clients",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "ClientGroups",
                type: "varchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AbstractClients",
                columns: table => new
                {
                    InternalId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbstractClients", x => x.InternalId);
                    table.UniqueConstraint("AK_AbstractClients_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbstractClients_ClientGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ClientGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Id",
                table: "Clients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AbstractClients_GroupId",
                table: "AbstractClients",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_AbstractClients_Id",
                table: "Clients",
                column: "Id",
                principalTable: "AbstractClients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_AbstractClients_Id",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "AbstractClients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Id",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "ClientGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Clients",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "GroupId",
                table: "Clients",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_GroupId",
                table: "Clients",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_ClientGroups_GroupId",
                table: "Clients",
                column: "GroupId",
                principalTable: "ClientGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
