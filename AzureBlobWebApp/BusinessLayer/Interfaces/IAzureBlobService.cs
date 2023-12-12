using System.Threading.Tasks;
using Azure.Storage.Blobs;
using AzureBlobWebApp.BusinessLayer.DTOs;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IAzureBlobService
    {
        Task CreateContainerIfNotExistsAsync(string userName);
        Task<List<Blob>> GetAllBlobs(string username);
        //Task UploadFileAsync(BlobContainerClient containerClient, string localFilePath);
        Task<BlobResponse> UploadAsync(string username, IFormFile file);
        Task<Blob?> DownloadAsync(string blobName, string username);
        Task<BlobResponse> DeleteAsync(string blobName, string username);
        Task<IEnumerable<string>> ListBlobContainersAsync();
    }
}
