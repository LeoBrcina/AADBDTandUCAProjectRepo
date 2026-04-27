using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicGramWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageLimits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxDownloadsPerMonth",
                table: "PackagePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MaxStorageBytes",
                table: "PackagePlans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "MaxUploadsPerMonth",
                table: "PackagePlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxDownloadsPerMonth",
                table: "PackagePlans");

            migrationBuilder.DropColumn(
                name: "MaxStorageBytes",
                table: "PackagePlans");

            migrationBuilder.DropColumn(
                name: "MaxUploadsPerMonth",
                table: "PackagePlans");
        }
    }
}
