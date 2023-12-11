namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IAzureBlobService
    {
        Task CreateContainer(string userName);
        Task<IEnumerable<string>> ListBlobContainersAsync();
    }
}
