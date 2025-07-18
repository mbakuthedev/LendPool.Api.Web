using System;
using System.ComponentModel.DataAnnotations.Schema;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class ReconciliationItem : BaseEntity
    {
        public string ReconciliationId { get; set; }
        public string FundUsageId { get; set; }
        public string DisbursementId { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public string? VerificationStatus { get; set; } // e.g. Pending, Verified, etc.
        public string? LenderComments { get; set; }
        public string? BorrowerComments { get; set; }
        public bool? IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
        public string? VerifiedBy { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? ReceiptUrl { get; set; }
        public bool? IsCompliant { get; set; } = false;
        public string? ComplianceNotes { get; set; }
        public DateTime Timestamp { get; set; } // Added for audit trail
        // Navigation properties
        [ForeignKey(nameof(ReconciliationId))]
        public LoanReconciliation Reconciliation { get; set; }
        [ForeignKey(nameof(FundUsageId))]
        public FundUsage FundUsage { get; set; }
        [ForeignKey(nameof(DisbursementId))]
        public Disbursement Disbursement { get; set; }
    }
} 