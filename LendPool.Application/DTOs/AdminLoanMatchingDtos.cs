namespace LendPool.Application.DTOs
{
    public class UnmatchedLoanRequestDto
    {
        public string LoanRequestId { get; set; }
        public string BorrowerName { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public int DurationMonths { get; set; }
    }

    public class LenderPoolDto
    {
        public string PoolId { get; set; }
        public string PoolName { get; set; }
    }

    public class AssignPoolRequestDto
    {
        public string PoolId { get; set; }
    }
} 