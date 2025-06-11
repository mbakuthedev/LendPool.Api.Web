using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Models
{
    public class LenderPoolMembership : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string LenderPoolId { get; set; }

        [ForeignKey(nameof(LenderPoolId))]
        public LenderPool LenderPool { get; set; }
        public DateTime JoinedAt { get; set; }


    }
}
