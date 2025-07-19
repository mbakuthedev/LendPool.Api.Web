using System;
using System.ComponentModel.DataAnnotations.Schema;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class Vote : BaseEntity
    {
        public string LoanRequestId { get; set; }
        public string LenderId { get; set; }
        public string VoteType { get; set; }
        public string? Comment { get; set; }
        public DateTime VotedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(LoanRequestId))]
        public LoanRequest LoanRequest { get; set; }

        [ForeignKey(nameof(LenderId))]
        public User Lender { get; set; }
    }
} 