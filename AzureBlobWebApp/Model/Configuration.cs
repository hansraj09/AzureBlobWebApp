using System;
using System.Collections.Generic;

namespace AzureBlobWebApp.Model;

public partial class Configuration
{
    public int ConfigId { get; set; }

    public string ConfigName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }
}
