using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniFinderWebProg.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MatchCount",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "UserPhotos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatchCount",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoPath",
                table: "UserPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
