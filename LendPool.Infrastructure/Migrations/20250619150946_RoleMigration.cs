using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoleMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "LoanRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenureInDays",
                table: "LoanRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "LenderPoolMemberships",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "TenureInDays",
                table: "LoanRequests");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "LenderPoolMemberships");
        }
    }
}
