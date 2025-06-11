using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class Repayment : BaseEntity
    {
        public string LoanId { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentChannel { get; set; } // e.g. "Card", "Bank Transfer"
        public decimal RemainingLoanBalance { get; set; }
        public bool IsFullyRepaid { get; set; }
        public decimal LateFee { get; set; }
        public bool IsLate { get; set; }

        public string LenderpoolId { get; set; }

        [ForeignKey(nameof(LenderpoolId))]
        public LenderPool LenderPool { get; set; }
        public Loan Loan { get; set; }
    }
}
