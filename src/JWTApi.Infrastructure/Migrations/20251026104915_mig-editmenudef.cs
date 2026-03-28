using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migeditmenudef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Controller",
                table: "Menus");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Menus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Menus");

            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Controller",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
