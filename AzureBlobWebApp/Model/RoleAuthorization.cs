using System;
using System.Collections.Generic;

namespace AzureBlobWebApp.Model;

public partial class RoleAuthorization
{
    public int AuthorizationId { get; set; }

    public int RoleId { get; set; }

    public virtual Authorization Authorization { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
