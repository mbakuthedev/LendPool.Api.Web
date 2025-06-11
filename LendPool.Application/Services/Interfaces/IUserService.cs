using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LendPool.Domain.Models;

namespace LendPool.Application.Services.Interfaces
{
    public interface IUserService
    {
      Task<User> GetUserByIdAsync(string userId);
    }


}
