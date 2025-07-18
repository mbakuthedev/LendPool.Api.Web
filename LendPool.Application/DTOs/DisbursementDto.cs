using System;
using System.Collections.Generic;
using LendPool.Domain.Enums;

namespace LendPool.Application.DTOs
{
    public class DisbursementDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string BorrowerId { get; set; }
        public string LenderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DisbursementDate { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public List<FundUsageDto> FundUsages { get; set; }
    }
} 