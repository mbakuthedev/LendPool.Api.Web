using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LendPool.Application.Extensions
{

        public static class ClaimsPrincipalExtension
        {
            public static string GetUserId(this ClaimsPrincipal user)
                => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

    
}
