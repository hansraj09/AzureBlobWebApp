using System.Net;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly IAzureBlobService _azureBlobService;
        private readonly ITokenService _tokenService;

        public BlobController(IAzureBlobService azureBlobService, ITokenService tokenService)
        {
            _azureBlobService = azureBlobService;
            _tokenService = tokenService;
        }


        [Route("GetContainers")]
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GetAllContainers()
        {
            var response = await _azureBlobService.ListBlobContainersAsync();
            return Ok(response);
        }

        [Route("GetBlobs")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBlobs()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }

            var response = _azureBlobService.GetAllBlobs(username);
            if (response == null)
            {
                return StatusCode(500, "Internal server error when loading files");
            }
            return Ok(response);
        }


        [HttpPost]
        [Authorize]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
            // look into chunks upload
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

        [HttpPost]
        [Authorize]
        [Route("Download")]
        public async Task<IActionResult> Download([FromBody] string GUID)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var username = _tokenService.GetUsername(token);
                if (username == null)
                {
                    return Unauthorized();
                }
                var result = await _azureBlobService.DownloadAsync(GUID, username);
                if (result == null)
                {
                    return BadRequest("File corrupted or does not exist");
                }
                return File(result.Content, result.ContentType, result.Name);
            }
            catch (Azure.RequestFailedException)
            {
                return StatusCode(500, "Download failed");
            }
        }

        [HttpDelete]
        [Authorize]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromBody] string GUID) 
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
            var response = _azureBlobService.Delete(GUID, username);
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
