using System.Net;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IAzureBlobService _azureBlobService;

        public BlobController(IAzureBlobService azureBlobService)
        {
            _azureBlobService = azureBlobService;
        }

        [Route("GetContainers")]
        [HttpGet]
        public async Task<IActionResult> GetAllContainers()
        {
            var response = await _azureBlobService.ListBlobContainersAsync();
            return Ok(response);
        }

        // ************************ Get token from body ***********************
        // authenticate token for user
        // get username from token


        [HttpPost]
        public async Task<IActionResult> Upload(string username, IFormFile file)
        {
            var response = await _azureBlobService.UploadAsync(username, file);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response);
            } else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, "Upload failed");
            } 
            return BadRequest(response);
        }

        [HttpGet]
        [Route("filename")]
        public async Task<IActionResult> Download(string username, string filename)
        {
            try
            {
                var result = await _azureBlobService.DownloadAsync(filename, username);
                if (result == null)
                {
                    return BadRequest("File corrupted or does not exist");
                }
                return File(result.Content, result.ContentType, result.Name);
            }
            catch (Azure.RequestFailedException ex)
            {
                return StatusCode(500, "Download failed");
            }
        }

        [HttpDelete]
        [Route("filename")]
        public async Task<IActionResult> Delete(string filename, string username) 
        {
            var response = await _azureBlobService.DeleteAsync(filename, username);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok("File successfully deleted");
            } else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, "Delete failed");
            }
            return BadRequest(response);
        }
    }
}
