using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicGramWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoFileSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Photos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Photos");
        }
    }
}
