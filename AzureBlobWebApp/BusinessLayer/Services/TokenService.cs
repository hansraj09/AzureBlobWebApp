using System.IdentityModel.Tokens.Jwt;
using AzureBlobWebApp.BusinessLayer.Interfaces;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class TokenService: ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        public TokenService(ILogger<TokenService> logger)
        {
            _logger = logger;
        }
        public string? GetUsername(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
            var username = securityToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            if (username == null)
            {
                _logger.Log(LogLevel.Error, "Could not get username from token.  Possible incorrect token");
            }
            return username;
        }
    }
}
