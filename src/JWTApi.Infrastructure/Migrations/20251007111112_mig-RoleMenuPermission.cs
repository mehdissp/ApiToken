using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migRoleMenuPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "RoleMenus",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_PermissionId",
                table: "RoleMenus",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenus_Permissions_PermissionId",
                table: "RoleMenus",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenus_Permissions_PermissionId",
                table: "RoleMenus");

            migrationBuilder.DropIndex(
                name: "IX_RoleMenus_PermissionId",
                table: "RoleMenus");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "RoleMenus");
        }
    }
}
