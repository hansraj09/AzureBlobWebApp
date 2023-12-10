using System.Net;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureBlobWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CJWTSetting _setting;
        private readonly ILoginService _loginService;

        public UserController(IOptions<CJWTSetting> options, ILoginService loginService)
        {
            _setting = options.Value;
            _loginService = loginService;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] CUserCredential userCred)
        {
            try
            {
                var response = _loginService.Authenticate(userCred);
                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    return BadRequest(response.StatusMessage);
                }

                return Ok(new CTokenResponse() { JWTToken = response.JWTToken, RefreshToken = response.RefreshToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {
            try
            {
                var response = _loginService.Refresh(token);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return NotFound(token.StatusMessage);
                }
                return Ok(new CTokenResponse() { JWTToken = response.JWTToken, RefreshToken = response.RefreshToken });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }      
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] User userInfo)
        {
            try
            {
                var response = _loginService.Register(userInfo);
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    return Conflict(response.StatusMessage);
                }
                return Ok("User has been successfully registered");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // Basic endpoint to demonstrate authorization
        // if the user is authenticated, then display the User table
        // TO BE REMOVED LATER
        [Authorize(Roles = "admin")]
        [Route("GetUsers")]
        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return _loginService.GetAllUsers();
        }
    }
}
