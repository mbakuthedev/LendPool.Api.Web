using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class Wallet : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "NGN";
        public List<WalletTransaction> Transactions { get; set; } = new();
        public User User { get; set; }
    }
}
