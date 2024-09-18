using Application.IService;
using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("Allow")]
    [Route("api/authentication")]
    [ApiController]
    public class AuthenController : BaseController
    {
        private readonly IAuthenService _authenticationService;
        public AuthenController(IAuthenService authen)
        {
            _authenticationService = authen;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerObject)
        {
            var result = await _authenticationService.RegisterAsync(registerObject);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
