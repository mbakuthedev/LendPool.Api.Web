namespace LendPool.Application.DTOs
{
    public class FundedLoanDto
    {
        public string Borrower { get; set; }
        public decimal Amount { get; set; }
        public string Pool { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }

    public class LenderRepaymentDto
    {
        public string Loan { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
        public string Status { get; set; } // paid, late, pending
    }

    public class LenderEarningsResponse
    {
        public decimal TotalEarnings { get; set; }
        public List<LenderRepaymentDto> Repayments { get; set; }
    }

    public class PoolPerformanceDto
    {
        public string Pool { get; set; }
        public decimal PerformanceValue { get; set; }
    }
} 