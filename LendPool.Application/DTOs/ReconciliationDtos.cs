using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LendPool.Domain.Enums;

namespace LendPool.Application.DTOs
{
    public class CreateReconciliationRequestDto
    {
        [Required]
        public string LoanId { get; set; }
        
        [Required]
        public string BorrowerId { get; set; }
        
        public string Comments { get; set; }
    }

    public class ReconciliationResponseDto
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string BorrowerId { get; set; }
        public string LenderId { get; set; }
        public string ReconciliationStatus { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Comments { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
        public decimal TotalVerifiedAmount { get; set; }
        public decimal DiscrepancyAmount { get; set; }
        public string DiscrepancyReason { get; set; }
        public bool IsCompliant { get; set; }
        public string ComplianceNotes { get; set; }
        public List<ReconciliationItemDto> Items { get; set; } = new();
    }

    public class ReconciliationItemDto
    {
        public string Id { get; set; }
        public string FundUsageId { get; set; }
        public string DisbursementId { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string VerificationStatus { get; set; }
        public string LenderComments { get; set; }
        public string BorrowerComments { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        public string VerifiedBy { get; set; }
        public string AttachmentUrl { get; set; }
        public string ReceiptUrl { get; set; }
        public bool IsCompliant { get; set; }
        public string ComplianceNotes { get; set; }
    }

    public class UpdateReconciliationItemDto
    {
        [Required]
        public string ItemId { get; set; }
        
        public string VerificationStatus { get; set; }
        public string LenderComments { get; set; }
        public bool IsVerified { get; set; }
        public bool IsCompliant { get; set; }
        public string ComplianceNotes { get; set; }
    }

    public class ReconciliationSummaryDto
    {
        public string LoanId { get; set; }
        public string BorrowerName { get; set; }
        public decimal TotalDisbursed { get; set; }
        public decimal TotalVerified { get; set; }
        public decimal DiscrepancyAmount { get; set; }
        public string ReconciliationStatus { get; set; }
        public bool IsCompliant { get; set; }
        public int TotalItems { get; set; }
        public int VerifiedItems { get; set; }
        public int PendingItems { get; set; }
        public int DisputedItems { get; set; }
    }

    public class FundUsageVerificationDto
    {
        [Required]
        public string FundUsageId { get; set; }
        
        [Required]
        public string VerificationStatus { get; set; }
        
        public string Comments { get; set; }
        public string ReceiptUrl { get; set; }
        public bool IsCompliant { get; set; }
        public string ComplianceNotes { get; set; }
    }
} 