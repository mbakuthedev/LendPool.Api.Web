using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.DTOs
{
    public class CreateLenderPoolDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
    }

    public class EditPoolInformationDto
    {
        public string Description { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
        public string  Rules { get; set; }

    }
    public class ContributeToPoolDto
    {
        public string PoolId { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
    }

    public class WithdrawFromPoolDto
    {
        public string PoolId { get; set; }
        public string UserId { get; set; }
        public string UserWalletId { get; set; }
        public decimal Amount { get; set; }
    }
}
