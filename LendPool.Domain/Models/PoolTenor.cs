using System;
using System.Collections.Generic;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class PoolTenor : BaseEntity
    {
        public string PoolId { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpectedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal TotalPoolAmount { get; set; }
        public decimal TotalLentAmount { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalLoss { get; set; }
        public string Status { get; set; }
        public List<Loan> Loans { get; set; }
        public List<LenderInvestment> LenderInvestments { get; set; }
    }
} 