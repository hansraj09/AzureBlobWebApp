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
    }
}
