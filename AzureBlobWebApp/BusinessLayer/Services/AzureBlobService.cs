using Azure.Storage;
using Azure.Storage.Blobs;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Repositories;
using Microsoft.Extensions.Options;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class AzureBlobService: IAzureBlobService
    {
        private readonly AzureBlobSetting _setting;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IDataRepository _dataRepository;
        public AzureBlobService(IOptions<AzureBlobSetting> options, IDataRepository dataRepository)
        {
            _setting = options.Value;
            _dataRepository = dataRepository;

            var credentials = new StorageSharedKeyCredential(_setting.StorageAccount, _setting.AccessKey);
            var accountUri = $"https://{_setting.StorageAccount}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(accountUri), credentials);
        }

        public async Task CreateContainer(string userName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(userName);
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        public async Task<IEnumerable<string>> ListBlobContainersAsync()
        {
            var containers = _blobServiceClient.GetBlobContainersAsync();
            var list = new List<string>();

            await foreach (var container in containers)
            {
                list.Add(container.Name);
            }
            return list;
        }

    }
}
