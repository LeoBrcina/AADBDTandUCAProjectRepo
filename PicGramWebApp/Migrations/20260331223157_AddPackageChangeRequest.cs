using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicGramWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageChangeRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageChangeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrentPackagePlanId = table.Column<int>(type: "int", nullable: false),
                    RequestedPackagePlanId = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApplied = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageChangeRequests_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageChangeRequests_PackagePlans_CurrentPackagePlanId",
                        column: x => x.CurrentPackagePlanId,
                        principalTable: "PackagePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PackageChangeRequests_PackagePlans_RequestedPackagePlanId",
                        column: x => x.RequestedPackagePlanId,
                        principalTable: "PackagePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageChangeRequests_CurrentPackagePlanId",
                table: "PackageChangeRequests",
                column: "CurrentPackagePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageChangeRequests_RequestedPackagePlanId",
                table: "PackageChangeRequests",
                column: "RequestedPackagePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageChangeRequests_UserId",
                table: "PackageChangeRequests",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageChangeRequests");
        }
    }
}
