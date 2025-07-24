using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LendPool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GeneralizeVotingSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_LoanRequests_LoanRequestId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_LoanRequestId",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "LoanRequestId",
                table: "Votes",
                newName: "OperationId");

            migrationBuilder.AlterColumn<string>(
                name: "VoteType",
                table: "Votes",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "OperationType",
                table: "Votes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OperationType",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "OperationId",
                table: "Votes",
                newName: "LoanRequestId");

            migrationBuilder.AlterColumn<int>(
                name: "VoteType",
                table: "Votes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_LoanRequestId",
                table: "Votes",
                column: "LoanRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_LoanRequests_LoanRequestId",
                table: "Votes",
                column: "LoanRequestId",
                principalTable: "LoanRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
