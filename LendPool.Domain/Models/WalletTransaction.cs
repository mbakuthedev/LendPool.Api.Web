using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class WalletTransaction : BaseEntity
    {
        public string WalletId { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Reference { get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; }
    }

}
