using System;
using LendPool.Domain.Models;

namespace LendPool.Application.DTOs
{
    public class LenderPoolJoinRequestDto
    {
        public string Id { get; set; }
        public string PoolId { get; set; }
        public string LenderId { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
    }
} 