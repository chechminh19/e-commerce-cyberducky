﻿using Application.IService;
using Application.ViewModels.UserDTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_cyberDucky.Controllers
{
    [EnableCors("AllowAll")]
    [Route("api/login")]
    [ApiController]
    public class AuthenController : BaseController
    {
        private readonly IAuthenService _authenticationService;
        public AuthenController(IAuthenService authen)
        {
            _authenticationService = authen;
        }
        /// <summary>
        /// register for customer
        /// </summary>      
        [AllowAnonymous]
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
        }/// <summary>
         /// login
         /// </summary>      
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginUserDTO loginObject)
        {
            var result = await _authenticationService.LoginAsync(loginObject);

            if (!result.Success)
            {
                return StatusCode(401, result);
            }
            else
            {
                return Ok(
                    new
                    {
                        success = result.Success,
                        message = result.Message,
                        token = result.DataToken,
                        role = result.Role,
                        hint = result.HintId,
                    }
                );
            }
        }
       


    }
}
