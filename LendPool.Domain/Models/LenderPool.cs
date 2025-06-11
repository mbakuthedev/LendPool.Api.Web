using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class LenderPool : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CreatedByUserId { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumAmount { get; set; }
        public decimal MaximumAmount { get; set; }
        public decimal TotalCapital { get; set; }
       
        [ForeignKey(nameof(CreatedByUserId))]
        public User CreatedByUser { get; set; }
        public ICollection<PoolContribution> Contributions { get; set; }
        public ICollection<LenderPoolMembership> LenderPoolMemberships { get; set; }

    }
}
