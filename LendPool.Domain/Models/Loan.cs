using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Enums;

namespace LendPool.Domain.Models
{
    public class Loan  : BaseEntity
    {
        public string LoanRequestId { get; set; }
        public string PoolId { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime DueDate { get; set; }
        public string LoanStatus { get; set; }

        public LoanRequest LoanRequest { get; set; }
        public LenderPool Pool { get; set; }
    }
}
