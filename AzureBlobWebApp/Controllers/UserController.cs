using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AzureBlobWebApp.Models;
using AzureBlobWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // include fetches the roles from the Role table with matching fields in UserRole table
            // filter first then fetch the data from other tables with foreign key references to avoid querying all users
            // or use lazy loading by adding .UseLazyLoadingProxies() in Program.cs
            var _user = _context.Users
                                .Where(user => user.UserName == userCred.UserName && user.Password == userCred.Password)
                                //.Include(user => user.Roles)
                                .FirstOrDefault();
            if (_user == null)
            {
                return Unauthorized("Incorrect username or password");
            }

            //var roles = _context.Users.Where(u => u.UserName == userCred.UserName).Single().Roles;
            //var serializedRoles = JsonSerializer.Serialize(roles);

            var _roles = _user.Roles.Select(role => role.RoleName);
            var serializedRoles = JsonSerializer.Serialize(_roles);

            // create a new JWT token for the authenticated user
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_setting.SecurityKey);
            var claims = _roles.Select(_role => new Claim(ClaimTypes.Role, _role));
            claims = claims.Append(new Claim(ClaimTypes.Name, _user.UserName));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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


            if (_reftable == null)
            {
                return Unauthorized("UserId or Refresh Token do not match on server");
            }
            TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
            return Ok(_result);
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] User userInfo)
        {
            try
            {
                var _user = _context.Users.FirstOrDefault(o => o.UserName == userInfo.UserName);
                if (_user != null)
                {
                    return Conflict("User already exists");
                }
                else
                {
                    User tblUser = new User()
                    {
                        UserName = userInfo.UserName,
                        Email = userInfo.Email,
                        Password = userInfo.Password,
                        
                    };
                    _context.Users.Add(tblUser);
                    _context.SaveChanges();
                    return Ok("User successfully created");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Basic endpoint to demonstrate authorization
        // if the user is authenticated, then display the User table
        [Authorize(Roles ="admin")]
        [Route("GetUsers")]
        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
