using System;
using LendPool.Domain.Enums;

namespace LendPool.Application.DTOs
{
    public class FundUsageDto
    {
        public string Id { get; set; }
        public string DisbursementId { get; set; }
        public string BorrowerId { get; set; }
        public decimal AmountUsed { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string AttachmentUrl { get; set; }
    }
} 