using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class Transaction : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }

        public User User { get; set; }
    }
}
