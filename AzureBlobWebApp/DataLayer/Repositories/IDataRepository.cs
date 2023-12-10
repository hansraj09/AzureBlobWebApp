using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;

namespace AzureBlobWebApp.DataLayer.Repositories
{
    public interface IDataRepository
    {
        int GetUserIdFromUsername(string username);
        User? GetUserByCredentials(CUserCredential userCred);
        IEnumerable<string> GetRoleNamesForUser(string username);
        ResponseBase RegisterNewUser(User userInfo);
        ResponseBase AddOrUpdateRefreshToken(string username, string token);
        RefreshToken? GetExistingRefreshTokenForUser(int userId, string token);

        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        IEnumerable<string> GetAllUsers();
    }
}
