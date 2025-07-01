using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LoanMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans");

            migrationBuilder.AlterColumn<string>(
                name: "LoanRequestId",
                table: "Loans",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "LoanRequests",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "LoanApprovals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoanRequestId = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApprovals_LoanRequests_LenderId",
                        column: x => x.LenderId,
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_LenderId",
                table: "LoanApprovals",
                column: "LenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "LoanApprovals");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "LoanRequests");

            migrationBuilder.AlterColumn<string>(
                name: "LoanRequestId",
                table: "Loans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id");
        }
    }
}
