using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migaddComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TodoId",
                table: "TodoStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Todos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TodoId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CommentId",
                table: "Users",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TodoStatuses_TodoId",
                table: "TodoStatuses",
                column: "TodoId");

            migrationBuilder.CreateIndex(
                name: "IX_Todos_CommentId",
                table: "Todos",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Todos_Comment_CommentId",
                table: "Todos",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoStatuses_Todos_TodoId",
                table: "TodoStatuses",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Comment_CommentId",
                table: "Users",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Comment_CommentId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_TodoStatuses_Todos_TodoId",
                table: "TodoStatuses");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Comment_CommentId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Users_CommentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_TodoStatuses_TodoId",
                table: "TodoStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CommentId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TodoId",
                table: "TodoStatuses");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tags");
        }
    }
}
