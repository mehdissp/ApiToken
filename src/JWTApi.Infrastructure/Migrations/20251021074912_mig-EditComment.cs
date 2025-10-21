using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWTApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class migEditComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Todos_Comment_CommentId",
                table: "Todos");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Comment_CommentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CommentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Todos_CommentId",
                table: "Todos");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Todos");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_TodoId",
                table: "Comment",
                column: "TodoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Todos_TodoId",
                table: "Comment",
                column: "TodoId",
                principalTable: "Todos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Todos_TodoId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_TodoId",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Todos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CommentId",
                table: "Users",
                column: "CommentId");

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
                name: "FK_Users_Comment_CommentId",
                table: "Users",
                column: "CommentId",
                principalTable: "Comment",
                principalColumn: "Id");
        }
    }
}
