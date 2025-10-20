using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migtodostatusOrderNum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderNum",
                table: "TodoStatuses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNum",
                table: "TodoStatuses");
        }
    }
}
