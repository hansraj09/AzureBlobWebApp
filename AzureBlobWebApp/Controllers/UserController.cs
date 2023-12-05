using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AzureBlobWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AzureBlobWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AzureBlobWebAppDbContext _context;
        private readonly JWTSetting _setting;

        public UserController(AzureBlobWebAppDbContext azureBlobWebAppDb, IOptions<JWTSetting> options) 
        {
            _context = azureBlobWebAppDb;
            _setting = options.Value;
        }

        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCredential userCred)
        {
            var _user = _context.Users.FirstOrDefault(user => user.UserName == userCred.UserName && user.Password == userCred.Password);
            if (_user == null)
            {
                return Unauthorized("Incorrect username or password");
            }

            // create a new JWT token for the authenticated user
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_setting.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, _user.UserName),
                    }
                ),
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenHandler.WriteToken(token);

            return Ok(finaltoken);
        }

        // Basic endpoint to demonstrate authorization
        // if the user is authenticated, then display the User table
        [Authorize]
        [Route("GetUsers")]
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }
    }
}
