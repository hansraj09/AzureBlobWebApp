namespace AzureBlobWebApp.DataLayer.Models;

public partial class Configuration
{
    public int ConfigId { get; set; }

    public string ConfigName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }

    public string? ConfigValue { get; set; }
}
