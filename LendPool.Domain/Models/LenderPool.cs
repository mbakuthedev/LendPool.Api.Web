using System;
using System.Collections.Generic;
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
       // public List<Lender> Lenders { get; set; } = new List<Lender>();
     
        public User CreatedByUser { get; set; }
        public ICollection<PoolContribution> Contributions { get; set; }

    }
}
