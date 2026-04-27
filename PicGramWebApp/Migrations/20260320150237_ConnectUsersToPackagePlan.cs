using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicGramWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ConnectUsersToPackagePlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PackagePlanId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PackagePlanId",
                table: "AspNetUsers",
                column: "PackagePlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PackagePlans_PackagePlanId",
                table: "AspNetUsers",
                column: "PackagePlanId",
                principalTable: "PackagePlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PackagePlans_PackagePlanId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PackagePlanId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PackagePlanId",
                table: "AspNetUsers");
        }
    }
}
