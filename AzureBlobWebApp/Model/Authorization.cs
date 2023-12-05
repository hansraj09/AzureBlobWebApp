using System;
using System.Collections.Generic;

namespace AzureBlobWebApp.Model;

public partial class Authorization
{
    public int AuthorizationId { get; set; }

    public string AuthorizationName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }
}
