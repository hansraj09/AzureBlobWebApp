using System.Security.Cryptography;
using AzureBlobWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureBlobWebApp.Services
{
    public class UserService: IUserService
    {
        private readonly AzureBlobWebAppDbContext _context;
        public UserService(AzureBlobWebAppDbContext dbContext)
        {
            _context = dbContext;
        }

        // get the userId from username
        // returns -1 if no user exists with given username which is an invalid Id
        public int GetUserIdFromUsername(string username)
        {
            var _user = _context.Users.FirstOrDefault(o => o.UserName == username);
            if (_user == null)
            {
                return -1;
            }
            return _user.UserId;
        }

        public string GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string RefreshToken = Convert.ToBase64String(randomnumber);

                var _userId = GetUserIdFromUsername(username);
                if (_userId == -1)
                {
                    throw new Exception("Username has no corresponding Id");
                }

                var _refreshUser = _context.RefreshTokens.FirstOrDefault(o => o.UserId == _userId);
                if (_refreshUser != null)
                {
                    _refreshUser.Token = RefreshToken;
                    _context.SaveChanges();
                }
                else
                {
                    RefreshToken tblRefreshtoken = new RefreshToken()
                    {
                        UserId = _userId,
                        Token = RefreshToken,
                        IsActive = true
                    };
                    _context.RefreshTokens.Add(tblRefreshtoken);
                    _context.SaveChanges();
                }

                return RefreshToken;
            }
        }
    }
}
