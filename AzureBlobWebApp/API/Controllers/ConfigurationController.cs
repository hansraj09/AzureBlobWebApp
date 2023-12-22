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
    }
}
