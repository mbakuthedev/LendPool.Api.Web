using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class InterestPayment : BaseEntity
    {
        public string PoolId { get; set; }

        [ForeignKey(nameof(PoolId))]
        public LenderPool Pool { get; set; }

        public string LoanId { get; set; }

        [ForeignKey(nameof(LoanId))]
        public Loan Loan { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaidByUserId { get; set; }

        [ForeignKey(nameof(PaidByUserId))]
        public User PaidByUser { get; set; }
    }

}
