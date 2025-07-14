using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace LendPool.Domain.Models
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
} 