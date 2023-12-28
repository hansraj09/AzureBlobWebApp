using System.Net;
using AzureBlobWebApp.BusinessLayer.DTOs;
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
        public async Task<IActionResult> Upload([FromForm] UploadFile fileInfo)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
            // look into chunks upload
            var response = await _azureBlobService.UploadAsync(username, fileInfo);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok(response);
            } else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, "Upload failed");
            } else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest(response.StatusMessage);
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
        public IActionResult Delete([FromBody] string GUID) 
        {
            var response = _azureBlobService.Delete(GUID);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok("File successfully deleted");
            } else if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, "Delete failed");
            }
            return BadRequest(response);
        }

        [HttpDelete]
        [Authorize]
        [Route("PermanentDelete")]
        public async Task<IActionResult> PermanentDelete([FromBody] string GUID)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
            var response = await _azureBlobService.PermanentDelete(GUID, username);
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, response.StatusMessage);
            } else if (response.StatusCode == HttpStatusCode.OK)
            {
                return Ok("File permanently deleted");
            }
            return BadRequest("Failed to permanently delete file");
        }

        [HttpPut]
        [Authorize]
        [Route("Restore")]
        public IActionResult Restore([FromBody] string GUID)
        {
            var response = _azureBlobService.Restore(GUID);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return BadRequest("Failed to restore file");
            }
            return Ok("File successfully restored");
        }
    }
}
