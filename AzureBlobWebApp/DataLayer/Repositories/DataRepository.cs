using System.Net;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using Microsoft.Extensions.Primitives;

namespace AzureBlobWebApp.DataLayer.Repositories
{
    public class DataRepository
    {
        private readonly AzureBlobWebAppDbContext _context;
        public DataRepository(AzureBlobWebAppDbContext dbcontext)
        {
            _context = dbcontext;
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

        public User? GetUserByCredentials(UserCredential userCred)
        {
            return _context.Users.Where(user => user.UserName == userCred.UserName && user.Password == userCred.Password).FirstOrDefault();
        }

        public IEnumerable<string> GetRoleNamesForUser(string username)
        {
            // include fetches the roles from the Role table with matching fields in UserRole table
            // filter first then fetch the data from other tables with foreign key references to avoid querying all users
            // or use lazy loading by adding .UseLazyLoadingProxies() in Program.cs
            var _user = _context.Users
                                .Where(user => user.UserName == username)
                                //.Include(user => user.Roles)
                                .FirstOrDefault();
            if (_user == null)
            {
                throw new MissingFieldException("Incorrect username");
            }
            return _user.Roles.Select(role => role.RoleName);
        }

        public ResponseBase AddOrUpdateRefreshToken(string username, string token)
        {
            var _userId = GetUserIdFromUsername(username);
            if (_userId == -1)
            {
                return new()
                {
                    StatusMessage = "Username has no corresponding Id",
                    StatusCode = HttpStatusCode.NoContent
                };
            }
            var _refreshUser = _context.RefreshTokens.FirstOrDefault(o => o.UserId == _userId);
            if (_refreshUser != null)
            {
                _refreshUser.Token = token;
                _context.SaveChanges();
            }
            else
            {
                RefreshToken tblRefreshtoken = new RefreshToken()
                {
                    UserId = _userId,
                    Token = token,
                    IsActive = true
                };
                _context.RefreshTokens.Add(tblRefreshtoken);
                _context.SaveChanges();
            }
            return new();
        }
    }
}
