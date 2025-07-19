using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LendPool.Domain.Enums;

namespace LendPool.Domain.DTOs
{
    public class CastVoteDto
    {
        [Required]
        public string LoanRequestId { get; set; }
        
        [Required]
        public VoteType VoteType { get; set; }
        
        public string? Comment { get; set; }
    }

    public class VoteResultDto
    {
        public string LoanRequestId { get; set; }
        public int TotalVotes { get; set; }
        public int ApproveVotes { get; set; }
        public int RejectVotes { get; set; }
        public int AbstainVotes { get; set; }
        public int TotalPoolMembers { get; set; }
        public decimal ApprovalPercentage { get; set; }
        public decimal RejectionPercentage { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public bool IsPending { get; set; }
        public List<VoteDetailDto> Votes { get; set; } = new();
    }

    public class VoteDetailDto
    {
        public string LenderId { get; set; }
        public string LenderName { get; set; }
        public VoteType VoteType { get; set; }
        public string? Comment { get; set; }
        public DateTime VotedAt { get; set; }
    }

    public class PoolVotingSummaryDto
    {
        public string PoolId { get; set; }
        public string PoolName { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveVoters { get; set; }
        public decimal ParticipationRate { get; set; }
        public List<VoteResultDto> RecentVotes { get; set; } = new();
    }
} 