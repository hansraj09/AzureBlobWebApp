using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureBlobWebApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        [Route("GetConfigs")]
        [HttpGet]
        [Authorize]
        public IActionResult GetConfigurations()
        {
            var response = _configurationService.GetConfigs();
            return Ok(response);
        }

        [Route("SetConfigs")]
        [HttpPut]
        [Authorize(Roles ="admin")]
        public IActionResult SetConfigurations([FromBody] CConfigs configs)
        {
            var response = _configurationService.SetConfigs(configs.maxSize, configs.allowedTypes);
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest(response.StatusMessage);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return StatusCode(500, response.StatusMessage);
            }
            return Ok("Configurations successfully set");
        }
    }
}
