using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using AzureBlobWebApp.DataLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class LoginService : ILoginService
    {
        private readonly CJWTSetting _setting;
        private readonly IDataRepository _dataRepository;
        public LoginService(IOptions<CJWTSetting> options, IDataRepository dataRepository)
        {
            _setting = options.Value;
            _dataRepository = dataRepository;
        }
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
            tokenResponse.RefreshToken = GenerateToken(username);

            return tokenResponse;
        }

        public TokenResponse Authenticate([FromBody] CUserCredential userCred)
        {
            var _user = _dataRepository.GetUserByCredentials(userCred);
            if (_user == null)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    StatusMessage = "Incorrect username or password"
                };
            }

            var _roles = _dataRepository.GetRoleNamesForUser(userCred.UserName);
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

            return new()
            {
                JWTToken = finaltoken,
                RefreshToken = GenerateToken(_user.UserName)
            };
        }

        public TokenResponse Refresh([FromBody] TokenResponse token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.JWTToken);
            var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var userId = _dataRepository.GetUserIdFromUsername(username);
            if (userId == -1)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    StatusMessage = "Username has no corresponding UserId"
                };
            }

            var _reftable = _dataRepository.GetExistingRefreshTokenForUser(userId, token.RefreshToken);
            if (_reftable == null)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    StatusMessage = "UserId or Refresh Token do not match on server"
                };
            }
            TokenResponse _result = Authenticate(username, securityToken.Claims.ToArray());
            return _result;
        }

        public ResponseBase Register([FromBody] User userInfo)
        {
            var _userId = _dataRepository.GetUserIdFromUsername(userInfo.UserName);
            if (_userId != -1)
            {
                return new ()
                {
                    StatusCode = HttpStatusCode.Conflict,
                    StatusMessage = "User already exists"
                };
            }
            else
            {
                var response = _dataRepository.RegisterNewUser(userInfo);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return response;
                }
                return new();
            }
        }

        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken = Convert.ToBase64String(randomnumber);

                var token = _dataRepository.AddOrUpdateRefreshToken(username, RefreshToken);

                if (token.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(token.StatusMessage);
                }

                return RefreshToken;
            }
        }

        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        public IEnumerable<string> GetAllUsers()
        {
            return _dataRepository.GetAllUsers();
        }
    }
}
