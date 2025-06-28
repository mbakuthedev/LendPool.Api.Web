
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.DTOs
{
    public class AdminWalletDto
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; } // optional
        public string Description { get; set; } // optional
    }

}
