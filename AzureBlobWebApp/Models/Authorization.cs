using System;
using System.Collections.Generic;

namespace AzureBlobWebApp.Models;

public partial class Authorization
{
    public int AuthorizationId { get; set; }

    public string AuthorizationName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
