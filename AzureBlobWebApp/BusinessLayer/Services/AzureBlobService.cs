using System.Net;
using System.Text.Json;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Repositories;
using Microsoft.Extensions.Options;
using File = AzureBlobWebApp.DataLayer.Models.File;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class AzureBlobService: IAzureBlobService
    {
        private readonly AzureBlobSetting _setting;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IDataRepository _dataRepository;
        private readonly ILogger<AzureBlobService> _logger;
        public AzureBlobService(IOptions<AzureBlobSetting> options, IDataRepository dataRepository, ILogger<AzureBlobService> logger)
        {
            _setting = options.Value;
            _dataRepository = dataRepository;
            _logger = logger;

            var credentials = new StorageSharedKeyCredential(_setting.StorageAccount, _setting.AccessKey);
            var accountUri = $"https://{_setting.StorageAccount}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(accountUri), credentials);
        }

        public async Task CreateContainerIfNotExistsAsync(string username)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(username);
            await blobContainerClient.CreateIfNotExistsAsync();
        }

        public List<Blob> GetAllBlobs(string username)
        {
            var dbFiles = _dataRepository.GetFiles(username);
            return dbFiles;           
        }

        private bool ValidateFileBeforeUpload(IFormFile file)
        {
            var configs = _dataRepository.GetConfigurations().ToArray();
            var maxSize = configs[0].ConfigValue;
            var allowedTypes = configs[1].ConfigValue;
            var deserializedTypes = JsonSerializer.Deserialize<List<string>>(allowedTypes);
            if (file.Length > (int.Parse(maxSize) * 1024 * 1024))
            {
                return false;
            }
            foreach (var type in deserializedTypes)
            {
                if (file.ContentType.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<BlobResponse> UploadAsync(string username, UploadFile fileInfo)
        {
            try
            {
                if (!ValidateFileBeforeUpload(fileInfo.file))
                {
                    _logger.Log(LogLevel.Error, "File to be uploaded does not match config values set");
                    return new()
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        StatusMessage = "File to be uploaded does not meet criteria"
                    };
                }
                await CreateContainerIfNotExistsAsync(username);
                var container = _blobServiceClient.GetBlobContainerClient(username);
                string guid = Guid.NewGuid().ToString();
                BlobClient client = container.GetBlobClient(guid);

                await using (Stream? data = fileInfo.file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                File fileDetails = new()
                {
                    FileName = fileInfo.file.FileName,
                    Type = fileInfo.file.ContentType,
                    Size = fileInfo.file.Length,
                    LastModified = DateTime.Now,
                    GUID = guid,
                    IsDeleted = false,
                    IsPublic = false,
                    Description = fileInfo.description
                };

                var dbResponse = _dataRepository.AddFile(username, fileDetails);
                if (dbResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    _logger.Log(LogLevel.Error, dbResponse.StatusMessage);
                    return new() { StatusCode = HttpStatusCode.InternalServerError, StatusMessage = "Internal Server Error" };
                }

                var response = new BlobResponse();
                //response.Blob.Uri = client.Uri.AbsoluteUri;
                //response.Blob.Name = client.Name;
                return response;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return new() { StatusCode = HttpStatusCode.InternalServerError, StatusMessage = ex.Message };
            }  
        }

        public async Task<DownloadFile?> DownloadAsync(string blobName, string username)
        {
            var container = _blobServiceClient.GetBlobContainerClient(username);
            BlobClient client = container.GetBlobClient(blobName);

            if (await client.ExistsAsync())
            {
                var content = await client.DownloadContentAsync();
                var filename = _dataRepository.GetFilenameFromGUID(blobName);

                return new DownloadFile()
                {
                    Content = content.Value.Content.ToStream(),
                    Name = filename,
                    ContentType = content.Value.Details.ContentType
                };
            }
            return null;
        }

        public ResponseBase Delete(string blobName)
        {
            
            var response = _dataRepository.DeleteFile(blobName);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Error, response.StatusMessage);
                return new() { StatusCode = HttpStatusCode.InternalServerError, StatusMessage = "Internal Server Error" };
            }

            return new();
        }

        public async Task<ResponseBase> PermanentDelete(string blobName, string username)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient(username);
                BlobClient client = container.GetBlobClient(blobName);
                await client.DeleteAsync();
                var dbResponse = _dataRepository.Removefile(blobName);
                if (dbResponse.StatusCode != HttpStatusCode.OK)
                {
                    _logger.Log(LogLevel.Error, dbResponse.StatusMessage);
                    return dbResponse;
                }
                return new();
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return new()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    StatusMessage = "Error while trying to delete on Azure Blob"
                };
            }
        }

        public ResponseBase Restore(string blobName)
        {
            var dbResponse = _dataRepository.RestoreFile(blobName);
            if (dbResponse.StatusCode != HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Error, dbResponse.StatusMessage);
                return dbResponse;
            }
            return new();
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
