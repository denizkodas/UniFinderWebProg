using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniFinderWebProg.Migrations
{
    /// <inheritdoc />
    public partial class UpddateUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches",
                column: "User1Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches",
                column: "User2Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches");

            migrationBuilder.DropForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches");

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User1Id",
                table: "Matches",
                column: "User1Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matches_Users_User2Id",
                table: "Matches",
                column: "User2Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
