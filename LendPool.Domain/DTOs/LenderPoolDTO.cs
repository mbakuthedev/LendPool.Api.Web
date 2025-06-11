using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.DTOs
{

        public class LenderPoolDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal InterestRate { get; set; }
            public decimal MinimumAmount { get; set; }
            public decimal MaximumAmount { get; set; }
        }

        public class PoolSummaryDto
        {
            public decimal TotalCapital { get; set; }
            public decimal TotalRepaid { get; set; }
            public decimal TotalEarnings { get; set; }
            public int ActiveLoansCount { get; set; }
        }

    }
