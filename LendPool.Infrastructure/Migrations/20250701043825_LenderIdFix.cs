using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LenderIdFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApprovals_LoanRequests_LenderId",
                table: "LoanApprovals");

            migrationBuilder.DropIndex(
                name: "IX_LoanApprovals_LenderId",
                table: "LoanApprovals");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_LoanRequestId",
                table: "LoanApprovals",
                column: "LoanRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApprovals_LoanRequests_LoanRequestId",
                table: "LoanApprovals",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApprovals_LoanRequests_LoanRequestId",
                table: "LoanApprovals");

            migrationBuilder.DropIndex(
                name: "IX_LoanApprovals_LoanRequestId",
                table: "LoanApprovals");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_LenderId",
                table: "LoanApprovals",
                column: "LenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApprovals_LoanRequests_LenderId",
                table: "LoanApprovals",
                column: "LenderId",
                principalTable: "LoanRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
