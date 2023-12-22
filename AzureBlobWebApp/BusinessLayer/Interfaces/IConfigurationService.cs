using AzureBlobWebApp.DataLayer.Models;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IConfigurationService
    {
        IEnumerable<Configuration> GetConfigs();
    }
}
