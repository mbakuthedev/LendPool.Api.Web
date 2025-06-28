using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class LoanApproval : BaseEntity
    {
        public string LoanRequestId { get; set; }
        public string LenderId { get; set; }
        public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(LenderId))]
        public LoanRequest LoanRequest { get; set; }
    }

}
