using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassIsland.ManagementServer.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileClassplans_ProfileGroups_GroupId",
                table: "ProfileClassplans");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSubjects_ProfileGroups_GroupId",
                table: "ProfileSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileTimelayouts_ProfileGroups_GroupId",
                table: "ProfileTimelayouts");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileTimelayouts",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileSubjects",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "ColorHex",
                table: "ProfileGroups",
                type: "varchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileClassplans",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileClassplans_ProfileGroups_GroupId",
                table: "ProfileClassplans",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSubjects_ProfileGroups_GroupId",
                table: "ProfileSubjects",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileTimelayouts_ProfileGroups_GroupId",
                table: "ProfileTimelayouts",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileClassplans_ProfileGroups_GroupId",
                table: "ProfileClassplans");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSubjects_ProfileGroups_GroupId",
                table: "ProfileSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileTimelayouts_ProfileGroups_GroupId",
                table: "ProfileTimelayouts");

            migrationBuilder.DropColumn(
                name: "ColorHex",
                table: "ProfileGroups");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileTimelayouts",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileSubjects",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "GroupId",
                table: "ProfileClassplans",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileClassplans_ProfileGroups_GroupId",
                table: "ProfileClassplans",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSubjects_ProfileGroups_GroupId",
                table: "ProfileSubjects",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileTimelayouts_ProfileGroups_GroupId",
                table: "ProfileTimelayouts",
                column: "GroupId",
                principalTable: "ProfileGroups",
                principalColumn: "Id");
        }
    }
}
