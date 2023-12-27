using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using File = AzureBlobWebApp.DataLayer.Models.File;

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
        IEnumerable<Configuration> GetConfigurations();
        ResponseBase SetConfigurations(int maxSize, string allowedTypes);
        int GetContainerIdFromUserId(int userId);
        ResponseBase AddFile(string username, File file);
        List<Blob> GetFiles(string username);
        string? GetFilenameFromGUID(string guid);
        ResponseBase DeleteFile(string guid);
        ResponseBase Removefile(string guid);
        ResponseBase RestoreFile(string guid);

        // this is a temporary method to test the authorization functionality
        // REMOVE LATER
        IEnumerable<string> GetAllUsers();
    }
}
