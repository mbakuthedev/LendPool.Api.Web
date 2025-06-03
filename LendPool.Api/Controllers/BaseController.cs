using Microsoft.AspNetCore.Mvc;

namespace LendPool.Api.Controllers
{
    [Route("lendpool/api/v1")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
 
  
            protected IActionResult Success(object data) => Ok(data);

            protected IActionResult NotFoundResponse(string message = "Not found") => NotFound(message);

            protected IActionResult BadRequestResponse(string message = "Bad request") => BadRequest(message);
        
    }
}
