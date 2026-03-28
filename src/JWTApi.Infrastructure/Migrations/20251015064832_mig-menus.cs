using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migmenus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraProject_Users_UserId",
                table: "ExtraProject");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPackage_Packages_PackageId",
                table: "UserPackage");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPackage_Users_UserId",
                table: "UserPackage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraProject",
                table: "ExtraProject");

            migrationBuilder.RenameTable(
                name: "UserPackage",
                newName: "UserPackages");

            migrationBuilder.RenameTable(
                name: "ExtraProject",
                newName: "ExtraProjects");

            migrationBuilder.RenameIndex(
                name: "IX_UserPackage_UserId",
                table: "UserPackages",
                newName: "IX_UserPackages_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPackage_PackageId",
                table: "UserPackages",
                newName: "IX_UserPackages_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_ExtraProject_UserId",
                table: "ExtraProjects",
                newName: "IX_ExtraProjects_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPackages",
                table: "UserPackages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraProjects",
                table: "ExtraProjects",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraProjects_Users_UserId",
                table: "ExtraProjects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackages_Users_UserId",
                table: "UserPackages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtraProjects_Users_UserId",
                table: "ExtraProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Packages_PackageId",
                table: "UserPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPackages_Users_UserId",
                table: "UserPackages");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPackages",
                table: "UserPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExtraProjects",
                table: "ExtraProjects");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Menus");

            migrationBuilder.RenameTable(
                name: "UserPackages",
                newName: "UserPackage");

            migrationBuilder.RenameTable(
                name: "ExtraProjects",
                newName: "ExtraProject");

            migrationBuilder.RenameIndex(
                name: "IX_UserPackages_UserId",
                table: "UserPackage",
                newName: "IX_UserPackage_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserPackages_PackageId",
                table: "UserPackage",
                newName: "IX_UserPackage_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_ExtraProjects_UserId",
                table: "ExtraProject",
                newName: "IX_ExtraProject_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPackage",
                table: "UserPackage",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExtraProject",
                table: "ExtraProject",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtraProject_Users_UserId",
                table: "ExtraProject",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackage_Packages_PackageId",
                table: "UserPackage",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPackage_Users_UserId",
                table: "UserPackage",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
