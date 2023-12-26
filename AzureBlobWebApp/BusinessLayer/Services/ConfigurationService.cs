using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;
using AzureBlobWebApp.DataLayer.Repositories;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly IDataRepository _dataRepository;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IDataRepository dataRepository, ILogger<ConfigurationService> logger)
        {
            _dataRepository = dataRepository;
            _logger = logger;
        }

        public IEnumerable<Configuration> GetConfigs()
        {
            var configs = _dataRepository.GetConfigurations();
            return configs;
        }

        public ResponseBase SetConfigs(int maxSize, string allowedTypes)
        {
            if (maxSize <= 0)
            {
                return new()
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    StatusMessage = "Max size cannot be less or equal to 0"
                };
            }
            var response = _dataRepository.SetConfigurations(maxSize, allowedTypes);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.Log(LogLevel.Error, response.StatusMessage);
                return new()
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    StatusMessage = "Could not find configurations"
                };
            }
            return response;
        }
    }
}
