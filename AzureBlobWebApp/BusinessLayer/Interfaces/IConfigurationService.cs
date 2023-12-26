using AzureBlobWebApp.DataLayer.DTOs;
using AzureBlobWebApp.DataLayer.Models;

namespace AzureBlobWebApp.BusinessLayer.Interfaces
{
    public interface IConfigurationService
    {
        IEnumerable<Configuration> GetConfigs();
        ResponseBase SetConfigs(int maxSize, string allowedTypes);
    }
}
