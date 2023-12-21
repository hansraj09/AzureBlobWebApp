﻿using System.Net;
using Azure.Storage;
using Azure.Storage.Blobs;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.Repositories;
using Microsoft.Extensions.Options;

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

        public async Task<List<Blob>?> GetAllBlobsAsync(string username)
        {
            try
            {
                await CreateContainerIfNotExistsAsync(username);
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(username);
                List<Blob> files = new List<Blob>();
                string containerUri = blobContainerClient.Uri.ToString();

                await foreach (var file in blobContainerClient.GetBlobsAsync())
                {
                    files.Add(new Blob
                    {
                        Uri = $"{blobContainerClient.Uri}/{file.Name}",
                        Name = file.Name,
                        ContentType = file.Properties.ContentType
                    });
                }
                return files;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return null;
            } 
            
        }

        public async Task<BlobResponse> UploadAsync(string username, IFormFile file)
        {
            try
            {
                await CreateContainerIfNotExistsAsync(username);
                var container = _blobServiceClient.GetBlobContainerClient(username);
                BlobClient client = container.GetBlobClient(file.FileName);

                await using (Stream? data = file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }
                var response = new BlobResponse();
                response.Blob.Uri = client.Uri.AbsoluteUri;
                response.Blob.Name = client.Name;
                return response;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return new() { StatusCode = HttpStatusCode.InternalServerError, StatusMessage = ex.Message };
            }  
        }

        public async Task<Blob?> DownloadAsync(string blobName, string username)
        {
            var container = _blobServiceClient.GetBlobContainerClient(username);
            BlobClient client = container.GetBlobClient(blobName);

            if (await client.ExistsAsync())
            {
                //var data = await client.OpenReadAsync();
                //Stream blobContent = data;

                var content = await client.DownloadContentAsync();

                return new Blob
                {
                    Content = content.Value.Content.ToStream(),
                    Name = blobName,
                    ContentType = content.Value.Details.ContentType
                };
            }
            return null;
        }

        public async Task<BlobResponse> DeleteAsync(string blobName, string username)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient(username);
                BlobClient client = container.GetBlobClient(blobName);

                await client.DeleteAsync();

                return new();
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return new() { StatusCode = HttpStatusCode.InternalServerError, StatusMessage = ex.Message };
            }
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
