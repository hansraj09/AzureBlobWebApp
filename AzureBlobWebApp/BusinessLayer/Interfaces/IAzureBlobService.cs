using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.DataLayer.DTOs;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IAzureBlobService
    {
        Task CreateContainerIfNotExistsAsync(string userName);
        List<Blob> GetAllBlobs(string username);
        Task<BlobResponse> UploadAsync(string username, IFormFile file);
        Task<DownloadFile?> DownloadAsync(string blobName, string username);
        ResponseBase Delete(string blobName);
        Task<ResponseBase> PermanentDelete(string blobName, string username);
        ResponseBase Restore(string blobName);
        Task<IEnumerable<string>> ListBlobContainersAsync();
    }
}
