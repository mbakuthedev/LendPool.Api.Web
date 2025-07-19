using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVotingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PoolTenorId",
                table: "Loans",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisbursementId",
                table: "FundUsages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FundUsages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "FundUsages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "BorrowerId",
                table: "FundUsages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentUrl",
                table: "FundUsages",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "FundUsages",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "ComplianceNotes",
                table: "FundUsages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "FundUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "FundUsages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompliant",
                table: "FundUsages",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "FundUsages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "FundUsages",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Disbursements",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "LoanId",
                table: "Disbursements",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "LenderId",
                table: "Disbursements",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Disbursements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BorrowerId",
                table: "Disbursements",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Disbursements",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "Disbursements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "Disbursements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LoanReconciliations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    BorrowerId = table.Column<string>(type: "text", nullable: false),
                    ReconciliationStatus = table.Column<string>(type: "text", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comments = table.Column<string>(type: "text", nullable: true),
                    TotalDisbursedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalVerifiedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscrepancyAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    DiscrepancyReason = table.Column<string>(type: "text", nullable: true),
                    IsCompliant = table.Column<bool>(type: "boolean", nullable: true),
                    ComplianceNotes = table.Column<string>(type: "text", nullable: true),
                    RequestedBy = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanReconciliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanReconciliations_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanReconciliations_User_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanReconciliations_User_LenderId",
                        column: x => x.LenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoolTenors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    DurationInMonths = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActualEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalPoolAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalLentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalProfit = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalLoss = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolTenors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoanRequestId = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    VoteType = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    VotedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_LoanRequests_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Votes_User_LenderId",
                        column: x => x.LenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReconciliationItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ReconciliationId = table.Column<string>(type: "text", nullable: false),
                    FundUsageId = table.Column<string>(type: "text", nullable: false),
                    DisbursementId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    VerificationStatus = table.Column<string>(type: "text", nullable: true),
                    LenderComments = table.Column<string>(type: "text", nullable: true),
                    BorrowerComments = table.Column<string>(type: "text", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: true),
                    VerifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VerifiedBy = table.Column<string>(type: "text", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "text", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "text", nullable: true),
                    IsCompliant = table.Column<bool>(type: "boolean", nullable: true),
                    ComplianceNotes = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReconciliationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReconciliationItems_Disbursements_DisbursementId",
                        column: x => x.DisbursementId,
                        principalTable: "Disbursements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReconciliationItems_FundUsages_FundUsageId",
                        column: x => x.FundUsageId,
                        principalTable: "FundUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReconciliationItems_LoanReconciliations_ReconciliationId",
                        column: x => x.ReconciliationId,
                        principalTable: "LoanReconciliations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LenderInvestments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    InvestmentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    InvestmentPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    InvestmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WithdrawalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WithdrawalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    ProfitEarned = table.Column<decimal>(type: "numeric", nullable: false),
                    LossIncurred = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    WithdrawalReason = table.Column<string>(type: "text", nullable: false),
                    IsEarlyWithdrawal = table.Column<bool>(type: "boolean", nullable: false),
                    EarlyWithdrawalPenalty = table.Column<decimal>(type: "numeric", nullable: false),
                    PoolTenorId = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderInvestments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LenderInvestments_PoolTenors_PoolTenorId",
                        column: x => x.PoolTenorId,
                        principalTable: "PoolTenors",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PoolTenorId",
                table: "Loans",
                column: "PoolTenorId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderInvestments_PoolTenorId",
                table: "LenderInvestments",
                column: "PoolTenorId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanReconciliations_BorrowerId",
                table: "LoanReconciliations",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanReconciliations_LenderId",
                table: "LoanReconciliations",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanReconciliations_LoanId",
                table: "LoanReconciliations",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_DisbursementId",
                table: "ReconciliationItems",
                column: "DisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_FundUsageId",
                table: "ReconciliationItems",
                column: "FundUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReconciliationItems_ReconciliationId",
                table: "ReconciliationItems",
                column: "ReconciliationId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_LenderId",
                table: "Votes",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_LoanRequestId",
                table: "Votes",
                column: "LoanRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_PoolTenors_PoolTenorId",
                table: "Loans",
                column: "PoolTenorId",
                principalTable: "PoolTenors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_PoolTenors_PoolTenorId",
                table: "Loans");

            migrationBuilder.DropTable(
                name: "LenderInvestments");

            migrationBuilder.DropTable(
                name: "ReconciliationItems");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "PoolTenors");

            migrationBuilder.DropTable(
                name: "LoanReconciliations");

            migrationBuilder.DropIndex(
                name: "IX_Loans_PoolTenorId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "PoolTenorId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ComplianceNotes",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "IsCompliant",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "FundUsages");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "Disbursements");

            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "Disbursements");

            migrationBuilder.AlterColumn<int>(
                name: "DisbursementId",
                table: "FundUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FundUsages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "FundUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "BorrowerId",
                table: "FundUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "AttachmentUrl",
                table: "FundUsages",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FundUsages",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Disbursements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LoanId",
                table: "Disbursements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "LenderId",
                table: "Disbursements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Disbursements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BorrowerId",
                table: "Disbursements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Disbursements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
