using LendPool.Application.Services.Implementation;
using System.Security.Claims;
using LendPool.Application.Services.Interfaces;
using LendPool.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    public class BorrowerController : BaseController
    {
        private readonly ILoanService _loanService;
        public BorrowerController(ILoanService loanService)
        {
            _loanService    = loanService;

        }

        //[HttpPost("lender/create-pool")]
        //public async Task<IActionResult> CreatePool([FromBody] CreateLenderPoolDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequestResponse("Invalid input data.");

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrWhiteSpace(userId))
        //        return Unauthorized("User is not authenticated.");

        //    var result = await _lenderPoolService.CreateLenderPoolAsync(dto, userId);

        //    if (!result.Success)
        //        return BadRequestResponse(result.Message);

        //    return Success(result);
        //}

    }
}
