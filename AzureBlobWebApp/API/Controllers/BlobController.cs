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


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
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
        [Authorize]
        [Route("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {
                var token = await HttpContext.GetTokenAsync("access_token");
                var username = _tokenService.GetUsername(token);
                if (username == null)
                {
                    return Unauthorized();
                }
                var result = await _azureBlobService.DownloadAsync(filename, username);
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
        [Route("filename")]
        public async Task<IActionResult> Delete(string filename) 
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var username = _tokenService.GetUsername(token);
            if (username == null)
            {
                return Unauthorized();
            }
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
