using HealthTracker.Api.Entities;
using HealthTracker.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealthTracker.Api.Controllers
{
    [Authorize]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticateModel model)
        {
            string token = await _userService.AuthenticateAsync(model.UserName, model.Password);

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            return Ok(token);
        }
    }
}
