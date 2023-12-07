using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AzureBlobWebApp.Models;
using AzureBlobWebApp.Services;
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
        private readonly IUserService _userService;

        public UserController(AzureBlobWebAppDbContext azureBlobWebAppDb, IOptions<JWTSetting> options, IUserService userService) 
        {
            _context = azureBlobWebAppDb;
            _setting = options.Value;
            _userService = userService;
        }

        [NonAction]
        public TokenResponse Authenticate(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(_setting.SecurityKey);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = _userService.GenerateToken(username);

            return tokenResponse;
        }


        [Route("Authenticate")]
        [HttpPost]
        public IActionResult Authenticate([FromBody] UserCredential userCred)
        {
            TokenResponse tokenResponse = new TokenResponse();
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
                        new Claim(ClaimTypes.Role, _user.Roles.ToString())
                    }
                ),
                Expires = DateTime.Now.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string finaltoken = tokenHandler.WriteToken(token);

            tokenResponse.JWTToken = finaltoken;
            tokenResponse.RefreshToken = _userService.GenerateToken(_user.UserName);

            return Ok(tokenResponse);
        }

        [Route("Refresh")]
        [HttpPost]
        public IActionResult Refresh([FromBody] TokenResponse token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.JWTToken);
            var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var userId = _userService.GetUserIdFromUsername(username);
            if (userId == -1)
            {
                throw new Exception("Username has no corresponding UserId");
            }


            //var username = principal.Identity.Name;
            var _reftable = _context.RefreshTokens.FirstOrDefault(o => o.UserId == userId && o.Token == token.RefreshToken);

            //var roles = _context.Users.Where(u => u.UserId == userId).Single().Roles;
            //var serializedRoles = JsonSerializer.Serialize(roles);

            if (_reftable == null)
            {
                return Unauthorized("UserId or Refresh Token do not match on server");
            }
            TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
            return Ok(_result);
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
