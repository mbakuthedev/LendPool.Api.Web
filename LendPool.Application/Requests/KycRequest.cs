using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Application.Requests
{
    public class BorrowerKycUpdateRequest
    {
        public string BVN { get; set; }
        public string NationalId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class LenderKycUpdateRequest
    {
        public string BusinessName { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal InvestmentCapacity { get; set; }
    }

}
