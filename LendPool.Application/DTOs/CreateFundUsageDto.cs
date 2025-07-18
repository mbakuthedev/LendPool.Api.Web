using System;
using LendPool.Domain.Enums;

namespace LendPool.Application.DTOs
{
    public class CreateFundUsageDto
    {
        public string DisbursementId { get; set; }
        public string BorrowerId { get; set; }
        public decimal AmountUsed { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string AttachmentUrl { get; set; }
    }
} 