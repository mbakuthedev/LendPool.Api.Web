using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PoolMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_LenderPools_LenderPoolId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_LenderPoolId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LenderPoolId",
                table: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LenderPoolId",
                table: "User",
                type: "text",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_User_LenderPools_LenderPoolId",
                table: "User",
                column: "LenderPoolId",
                principalTable: "LenderPools",
                principalColumn: "Id");
        }
    }
}
