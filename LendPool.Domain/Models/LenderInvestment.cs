using System;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class LenderInvestment : BaseEntity
    {
        public string LenderId { get; set; }
        public string PoolId { get; set; }
        public decimal InvestmentAmount { get; set; }
        public decimal InvestmentPercentage { get; set; }
        public DateTime InvestmentDate { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public decimal ProfitEarned { get; set; }
        public decimal LossIncurred { get; set; }
        public string Status { get; set; }
        public string WithdrawalReason { get; set; }
        public bool IsEarlyWithdrawal { get; set; }
        public decimal EarlyWithdrawalPenalty { get; set; }
    }
} 