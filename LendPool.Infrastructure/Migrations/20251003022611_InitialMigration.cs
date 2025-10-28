using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disbursements",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<string>(type: "text", nullable: false),
                    BorrowerId = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disbursements", x => x.Id);
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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    BVN = table.Column<string>(type: "text", nullable: true),
                    NIN = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    BusinessName = table.Column<string>(type: "text", nullable: true),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    InvestmentCapacity = table.Column<decimal>(type: "numeric", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsKycVerified = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentId = table.Column<string>(type: "text", nullable: true),
                    DocumentUrl = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FundUsages",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisbursementId = table.Column<string>(type: "text", nullable: false),
                    BorrowerId = table.Column<string>(type: "text", nullable: false),
                    AmountUsed = table.Column<decimal>(type: "numeric", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AttachmentUrl = table.Column<string>(type: "text", nullable: true),
                    VerificationStatus = table.Column<string>(type: "text", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "text", nullable: true),
                    IsCompliant = table.Column<bool>(type: "boolean", nullable: true),
                    ComplianceNotes = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FundUsages_Disbursements_DisbursementId",
                        column: x => x.DisbursementId,
                        principalTable: "Disbursements",
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

            migrationBuilder.CreateTable(
                name: "LenderPools",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Rules = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "text", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    MaximumAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalCapital = table.Column<decimal>(type: "numeric", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderPools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LenderPools_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    OperationId = table.Column<string>(type: "text", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    VoteType = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    VotedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_User_LenderId",
                        column: x => x.LenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LenderPoolJoinRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    LenderId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReviewedBy = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderPoolJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LenderPoolJoinRequests_LenderPools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LenderPoolJoinRequests_User_LenderId",
                        column: x => x.LenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LenderPoolMemberships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    LenderPoolId = table.Column<string>(type: "text", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LenderPoolMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LenderPoolMemberships_LenderPools_LenderPoolId",
                        column: x => x.LenderPoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LenderPoolMemberships_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanRequests",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    BorrowerId = table.Column<string>(type: "text", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    DurationInMonths = table.Column<int>(type: "integer", nullable: false),
                    RequestStatus = table.Column<string>(type: "text", nullable: false),
                    TenureInDays = table.Column<int>(type: "integer", nullable: false),
                    AdminComment = table.Column<string>(type: "text", nullable: true),
                    MatchedPoolId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanRequests_LenderPools_MatchedPoolId",
                        column: x => x.MatchedPoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoanRequests_User_BorrowerId",
                        column: x => x.BorrowerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PoolContributions",
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
                    table.PrimaryKey("PK_PoolContributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PoolContributions_LenderPools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PoolContributions_User_LenderId",
                        column: x => x.LenderId,
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
                        name: "FK_LoanApprovals_LoanRequests_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    PoolId = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalRepaid = table.Column<decimal>(type: "numeric", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LoanStatus = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoanRequestId = table.Column<string>(type: "text", nullable: false),
                    PoolTenorId = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Loans_LenderPools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_LoanRequests_LoanRequestId",
                        column: x => x.LoanRequestId,
                        principalTable: "LoanRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Loans_PoolTenors_PoolTenorId",
                        column: x => x.PoolTenorId,
                        principalTable: "PoolTenors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Loans_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "Repayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoanId = table.Column<string>(type: "text", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionReference = table.Column<string>(type: "text", nullable: false),
                    PaymentChannel = table.Column<string>(type: "text", nullable: false),
                    RemainingLoanBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    IsFullyRepaid = table.Column<bool>(type: "boolean", nullable: false),
                    LateFee = table.Column<decimal>(type: "numeric", nullable: false),
                    IsLate = table.Column<bool>(type: "boolean", nullable: false),
                    LenderpoolId = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateModified = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repayments_LenderPools_LenderpoolId",
                        column: x => x.LenderpoolId,
                        principalTable: "LenderPools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Repayments_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Address", "BVN", "BusinessName", "DateCreated", "DateModified", "DateOfBirth", "DocumentId", "DocumentUrl", "Email", "FirstName", "FullName", "InvestmentCapacity", "IsKycVerified", "LastName", "NIN", "PasswordHash", "RegistrationNumber", "Role" },
                values: new object[] { "11111111-1111-1111-1111-111111111111", null, null, null, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, "admin@lendpool.com", "Admin", "Admin Gbemidebe", null, false, "Gbemidebe", null, "$2a$11$.7M3DWXSh2PA6ETF4DyBLuKwDj1SAY7.aEfcj3a7x7q8ClPMT42bO", null, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_FundUsages_DisbursementId",
                table: "FundUsages",
                column: "DisbursementId");

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
                name: "IX_LenderInvestments_PoolTenorId",
                table: "LenderInvestments",
                column: "PoolTenorId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolJoinRequests_LenderId",
                table: "LenderPoolJoinRequests",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolJoinRequests_PoolId",
                table: "LenderPoolJoinRequests",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolMemberships_LenderPoolId",
                table: "LenderPoolMemberships",
                column: "LenderPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPoolMemberships_UserId",
                table: "LenderPoolMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LenderPools_CreatedByUserId",
                table: "LenderPools",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApprovals_LoanRequestId",
                table: "LoanApprovals",
                column: "LoanRequestId");

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
                name: "IX_LoanRequests_BorrowerId",
                table: "LoanRequests",
                column: "BorrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanRequests_MatchedPoolId",
                table: "LoanRequests",
                column: "MatchedPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LoanRequestId",
                table: "Loans",
                column: "LoanRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PoolId",
                table: "Loans",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_PoolTenorId",
                table: "Loans",
                column: "PoolTenorId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolContributions_LenderId",
                table: "PoolContributions",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PoolContributions_PoolId",
                table: "PoolContributions",
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
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_LenderpoolId",
                table: "Repayments",
                column: "LenderpoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Repayments_LoanId",
                table: "Repayments",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_LenderId",
                table: "Votes",
                column: "LenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterestPayments");

            migrationBuilder.DropTable(
                name: "LenderInvestments");

            migrationBuilder.DropTable(
                name: "LenderPoolJoinRequests");

            migrationBuilder.DropTable(
                name: "LenderPoolMemberships");

            migrationBuilder.DropTable(
                name: "LoanApprovals");

            migrationBuilder.DropTable(
                name: "PoolContributions");

            migrationBuilder.DropTable(
                name: "PoolWithdrawals");

            migrationBuilder.DropTable(
                name: "ReconciliationItems");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Repayments");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "FundUsages");

            migrationBuilder.DropTable(
                name: "LoanReconciliations");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Disbursements");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "LoanRequests");

            migrationBuilder.DropTable(
                name: "PoolTenors");

            migrationBuilder.DropTable(
                name: "LenderPools");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
