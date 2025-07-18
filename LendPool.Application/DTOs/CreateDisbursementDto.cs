using System;

namespace LendPool.Application.DTOs
{
    public class CreateDisbursementDto
    {
        public string LoanId { get; set; }
        public string BorrowerId { get; set; }
        public string LenderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
} 