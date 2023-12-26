using AzureBlobWebApp.BusinessLayer.DTOs;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IAzureBlobService
    {
        Task CreateContainerIfNotExistsAsync(string userName);
        List<Blob> GetAllBlobs(string username);
        Task<BlobResponse> UploadAsync(string username, IFormFile file);
        Task<DownloadFile?> DownloadAsync(string blobName, string username);
        BlobResponse Delete(string blobName, string username);
        Task<IEnumerable<string>> ListBlobContainersAsync();
    }
}
