using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class I : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LenderPoolId",
                table: "User",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LenderPoolMembership",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LenderPoolId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderPoolMembership", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LenderPoolMembership_LenderPools_LenderPoolId",
                        column: x => x.LenderPoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LenderPoolMembership_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "11111111-1111-1111-1111-111111111111",
                column: "LenderPoolId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_User_LenderPoolId",
                table: "User",
                column: "LenderPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolMembership_LenderPoolId",
                table: "LenderPoolMembership",
                column: "LenderPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolMembership_UserId",
                table: "LenderPoolMembership",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_LenderPools_LenderPoolId",
                table: "User",
                column: "LenderPoolId",
                principalTable: "LenderPools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_LenderPools_LenderPoolId",
                table: "User");

            migrationBuilder.DropTable(
                name: "LenderPoolMembership");

            migrationBuilder.DropIndex(
                name: "IX_User_LenderPoolId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LenderPoolId",
                table: "User");
        }
    }
}
