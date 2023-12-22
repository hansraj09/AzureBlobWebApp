using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.DataLayer.Models;
using AzureBlobWebApp.DataLayer.Repositories;

namespace AzureBlobWebApp.BusinessLayer.Services
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly IDataRepository _dataRepository;

        public ConfigurationService(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        public IEnumerable<Configuration> GetConfigs()
        {
            var configs = _dataRepository.GetConfigurations();
            return configs;
        }
    }
}
