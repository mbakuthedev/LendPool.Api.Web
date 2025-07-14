using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LendPool.Domain.Models
{
    public enum JoinRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class LenderPoolJoinRequest : BaseEntity
    {
        public string PoolId { get; set; }
        public string LenderId { get; set; }
        public string Status { get; set; } = JoinRequestStatus.Pending.ToString();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public string ReviewedBy { get; set; } // SuperLenderId

        [ForeignKey(nameof(PoolId))]
        public LenderPool Pool { get; set; }
        [ForeignKey(nameof(LenderId))]
        public User Lender { get; set; }
    }
} 