namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface ITokenService
    {
        string? GetUsername(string token);
    }
}
