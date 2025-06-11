using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.DTOs
{
    public class AddUserToPoolDto
    {
        public string UserId { get; set; }
        public string PoolId { get; set; }
    }

}
