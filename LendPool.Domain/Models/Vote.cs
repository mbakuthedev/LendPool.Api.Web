using System;
using System.ComponentModel.DataAnnotations.Schema;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class Vote : BaseEntity
    {
        public string OperationId { get; set; } // ID of the operation being voted on (loan, rule, etc.)
        public VoteOperationType OperationType { get; set; } // Type of operation
        public string LenderId { get; set; }
        public string VoteType { get; set; } // Approve, Reject, Abstain
        public string? Comment { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(LenderId))]
        public User Lender { get; set; }
    }
} 