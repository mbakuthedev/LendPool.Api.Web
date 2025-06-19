using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Domain.Enums
{
    public enum UserRole
    {
        Admin = 1,
        Lender = 2,
        SuperLender = 3,
        Borrower = 4
    }
}
