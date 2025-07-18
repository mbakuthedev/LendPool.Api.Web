using System;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class FundUsage : BaseEntity
    {
        public string DisbursementId { get; set; }
        public string BorrowerId { get; set; }
        public decimal AmountUsed { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string? AttachmentUrl { get; set; }

        // Reconciliation properties
        public string? VerificationStatus { get; set; } // e.g. Pending, Verified, etc.
        public string? ReceiptUrl { get; set; }
        public bool? IsCompliant { get; set; }
        public string? ComplianceNotes { get; set; }
    }
} 