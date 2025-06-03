using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class PoolContribution : BaseEntity
    {
        public string LenderId { get; set; }
        public string PoolId { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey(nameof(LenderId))]
        public User Lender { get; set; }

        [ForeignKey(nameof(PoolId))]
        public LenderPool Pool { get; set; }

    }
}
