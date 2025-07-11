﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class LoanRequest : BaseEntity
    {
        public string BorrowerId { get; set; }
        public decimal RequestedAmount { get; set; }
        public string Purpose { get; set; }
        public decimal Amount { get; set; }
        public int DurationInMonths { get; set; }
        public string RequestStatus { get; set; }
        public int TenureInDays { get; set; }
        public string? AdminComment { get; set; } // make this a list of comments later on, so everyone can comment 
        public string MatchedPoolId { get; set; }

        public User Borrower { get; set; }
        public LenderPool? MatchedPool { get; set; }
        public List<LoanApproval> Approvals { get; set; } = new();
    }
}
