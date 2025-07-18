using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class LoanReconciliation : BaseEntity
    {
        public string LoanId { get; set; }
        public string LenderId { get; set; }
        public string BorrowerId { get; set; }
        public string ReconciliationStatus { get; set; }
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }
        public string? Comments { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
        public decimal TotalVerifiedAmount { get; set; }
        public decimal DiscrepancyAmount { get; set; }
        public string? DiscrepancyReason { get; set; }
        public bool? IsCompliant { get; set; } = false;
        public string? ComplianceNotes { get; set; }
        public string RequestedBy { get; set; }

        // Navigation properties
        [ForeignKey(nameof(LoanId))]
        public Loan Loan { get; set; }
        
        [ForeignKey(nameof(LenderId))]
        public User Lender { get; set; }
        
        [ForeignKey(nameof(BorrowerId))]
        public User Borrower { get; set; }
        
        public List<ReconciliationItem> ReconciliationItems { get; set; } = new();
    }
} 