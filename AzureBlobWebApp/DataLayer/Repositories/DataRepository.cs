using System.Net;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

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

            // add container
            Container tblContainer = new()
            {
                ContainerName = userInfo.UserName,
                LastModified = DateTime.Now,
                ModifiedUserId = userInfo.UserId,
                UserId = userInfo.UserId 
            };
            _context.Containers.Add(tblContainer);
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
        public IEnumerable<Configuration> GetConfigurations()
        {
            return _context.Configurations.ToList();
        }

        public ResponseBase SetConfigurations(int maxSize, string allowedTypes)
        {
            var maxSizeRecord = _context.Configurations.FirstOrDefault(c => c.ConfigId == 1);
            if (maxSizeRecord != null)
            {
                maxSizeRecord.ConfigValue = maxSize.ToString();
            }
            else
            {
                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    StatusMessage = "Could not find maxSize record (id: 1)"
                };
            }
            var allowedTypesRecord = _context.Configurations.FirstOrDefault(c => c.ConfigId == 2);
            if (allowedTypesRecord != null)
            {
                allowedTypesRecord.ConfigValue = allowedTypes;
            }
            else
            {
                return new()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    StatusMessage = "Could not find allowedTypes record (id: 2)"
                };
            }
            _context.SaveChanges();
            return new();
        }



        public int GetContainerIdFromUserId(int userId)
        {
            var container =_context.Containers.FirstOrDefault(c => c.UserId == userId);
            if (container == null)
            {
                return -1;
            }
            return container.ContainerId;
        }

        public ResponseBase AddFile(string username, Models.File file)
        {
            var userId = GetUserIdFromUsername(username);
            if (userId == -1)
            {
                return new ResponseBase()
                {
                    StatusCode = HttpStatusCode.NoContent,
                    StatusMessage = "No user exists with this username"
                };
            }
            var containerId = GetContainerIdFromUserId(userId);
            if (containerId == -1)
            {
                return new ResponseBase()
                {
                    StatusCode = HttpStatusCode.NoContent,
                    StatusMessage = "No container found for the user"
                };
            }
            file.ModifiedUserId = userId;
            file.ContainerId = containerId;
            _context.Files.Add(file);
            _context.SaveChanges();
            return new();
        }

        public List<Blob> GetFiles(string username)
        {
            var userId = GetUserIdFromUsername(username);
            var containerId = GetContainerIdFromUserId(userId);
            var files = _context.Files.Where(f => f.ContainerId == containerId);
            List<Blob> fileList = new();
            foreach (var file in files)
            {
                fileList.Add(new Blob()
                {
                    ContainerId = file.ContainerId,
                    Description = file.Description,
                    FileId = file.FileId,
                    FileName = file.FileName,
                    GUID = file.GUID,
                    IsDeleted = file.IsDeleted,
                    IsPublic = file.IsPublic,
                    LastModified = file.LastModified,
                    ModifiedUserId = file.ModifiedUserId,
                    Size = file.Size,
                    Type = file.Type,
                });
            }
            return fileList;
        }

        public string? GetFilenameFromGUID(string guid)
        {
            return _context.Files.Select(f => f.FileName).FirstOrDefault();
        }

        public ResponseBase DeleteFile(string guid)
        {
            var fileToDelete = _context.Files.SingleOrDefault(f => f.GUID == guid);
            if (fileToDelete != null)
            {
                fileToDelete.IsDeleted = true;
                _context.SaveChanges();
                return new();
            }
            return new ResponseBase()
            {
                StatusCode = HttpStatusCode.NotFound,
                StatusMessage = "Could not find file to be deleted"
            };
        }

        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        public IEnumerable<string> GetAllUsers()
        {
            return _context.Users.Select(u => u.UserName).ToList();
        }
    }
}
