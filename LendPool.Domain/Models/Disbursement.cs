using System;
using System.Collections.Generic;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class Disbursement : BaseEntity
    {
        public string LoanId { get; set; }
        public string BorrowerId { get; set; }
        public string LenderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public List<FundUsage>? FundUsages { get; set; }
    }
} 