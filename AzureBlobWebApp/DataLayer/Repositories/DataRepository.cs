using System.Net;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;

namespace AzureBlobWebApp.DataLayer.Repositories
{
    public class DataRepository: IDataRepository
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

        public User? GetUserByCredentials(CUserCredential userCred)
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

        public ResponseBase RegisterNewUser(User userInfo)
        {
            // get 'user' role
            var _userRole = _context.Roles.Where(r => r.RoleId == 2).FirstOrDefault();
            if (_userRole == null)
            {
                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    StatusMessage = "No user role found"
                };
            }
            User tblUser = new()
            {
                UserName = userInfo.UserName,
                Email = userInfo.Email,
                Password = userInfo.Password,
                LastModified = DateTime.Now,
            };
            tblUser.Roles.Add(_userRole);
            _context.Users.Add(tblUser);
            _context.SaveChanges();
            return new();
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

        public RefreshToken? GetExistingRefreshTokenForUser(int userId, string token)
        {
            return _context.RefreshTokens.FirstOrDefault(o => o.UserId == userId && o.Token == token);
        }


        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        public IEnumerable<string> GetAllUsers()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
