using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PoolMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LenderPoolMembership_LenderPools_LenderPoolId",
                table: "LenderPoolMembership");

            migrationBuilder.DropForeignKey(
                name: "FK_LenderPoolMembership_User_UserId",
                table: "LenderPoolMembership");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LenderPoolMembership",
                table: "LenderPoolMembership");

            migrationBuilder.RenameTable(
                name: "LenderPoolMembership",
                newName: "LenderPoolMemberships");

            migrationBuilder.RenameIndex(
                name: "IX_LenderPoolMembership_UserId",
                table: "LenderPoolMemberships",
                newName: "IX_LenderPoolMemberships_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LenderPoolMembership_LenderPoolId",
                table: "LenderPoolMemberships",
                newName: "IX_LenderPoolMemberships_LenderPoolId");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Wallets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsFullyRepaid",
                table: "Repayments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLate",
                table: "Repayments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LateFee",
                table: "Repayments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "LenderpoolId",
                table: "Repayments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentChannel",
                table: "Repayments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingLoanBalance",
                table: "Repayments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Repayments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "LoanRequestId",
                table: "Loans",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Loans",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRepaid",
                table: "Loans",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Loans",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "LenderPoolMemberships",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_LenderPoolMemberships",
                table: "LenderPoolMemberships",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "InterestPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidByUserId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestPayments_LenderPools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestPayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterestPayments_User_PaidByUserId",
                        column: x => x.PaidByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoolWithdrawals",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolWithdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolWithdrawals_LenderPools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoolWithdrawals_User_LenderId",
                        column: x => x.LenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    WalletId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_LenderpoolId",
                table: "Repayments",
                column: "LenderpoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestPayments_LoanId",
                table: "InterestPayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestPayments_PaidByUserId",
                table: "InterestPayments",
                column: "PaidByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestPayments_PoolId",
                table: "InterestPayments",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolWithdrawals_LenderId",
                table: "PoolWithdrawals",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolWithdrawals_PoolId",
                table: "PoolWithdrawals",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_LenderPoolMemberships_LenderPools_LenderPoolId",
                table: "LenderPoolMemberships",
                column: "LenderPoolId",
                principalTable: "LenderPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LenderPoolMemberships_User_UserId",
                table: "LenderPoolMemberships",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_User_UserId",
                table: "Loans",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Repayments_LenderPools_LenderpoolId",
                table: "Repayments",
                column: "LenderpoolId",
                principalTable: "LenderPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LenderPoolMemberships_LenderPools_LenderPoolId",
                table: "LenderPoolMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_LenderPoolMemberships_User_UserId",
                table: "LenderPoolMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_User_UserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Repayments_LenderPools_LenderpoolId",
                table: "Repayments");

            migrationBuilder.DropTable(
                name: "InterestPayments");

            migrationBuilder.DropTable(
                name: "PoolWithdrawals");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Repayments_LenderpoolId",
                table: "Repayments");

            migrationBuilder.DropIndex(
                name: "IX_Loans_UserId",
                table: "Loans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LenderPoolMemberships",
                table: "LenderPoolMemberships");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "IsFullyRepaid",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "IsLate",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "LateFee",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "LenderpoolId",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "PaymentChannel",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "RemainingLoanBalance",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Repayments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "TotalRepaid",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "LenderPoolMemberships");

            migrationBuilder.RenameTable(
                name: "LenderPoolMemberships",
                newName: "LenderPoolMembership");

            migrationBuilder.RenameIndex(
                name: "IX_LenderPoolMemberships_UserId",
                table: "LenderPoolMembership",
                newName: "IX_LenderPoolMembership_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_LenderPoolMemberships_LenderPoolId",
                table: "LenderPoolMembership",
                newName: "IX_LenderPoolMembership_LenderPoolId");

            migrationBuilder.AlterColumn<string>(
                name: "LoanRequestId",
                table: "Loans",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LenderPoolMembership",
                table: "LenderPoolMembership",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LenderPoolMembership_LenderPools_LenderPoolId",
                table: "LenderPoolMembership",
                column: "LenderPoolId",
                principalTable: "LenderPools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LenderPoolMembership_User_UserId",
                table: "LenderPoolMembership",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LoanRequests_LoanRequestId",
                table: "Loans",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
