namespace AzureBlobWebApp.Services
{
    public interface IUserService
    {
        int GetUserIdFromUsername(string username);

        string GenerateToken(string username);
    }
}
