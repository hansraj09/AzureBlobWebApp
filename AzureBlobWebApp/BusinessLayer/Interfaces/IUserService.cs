namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IUserService
    {
        int GetUserIdFromUsername(string username);

        string GenerateToken(string username);
    }
}
