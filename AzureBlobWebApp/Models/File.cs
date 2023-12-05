using System;
using System.Collections.Generic;

namespace AzureBlobWebApp.Models;

public partial class File
{
    public int FileId { get; set; }

    public string FileName { get; set; } = null!;

    public string Type { get; set; } = null!;

    public decimal Size { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsPublic { get; set; }

    public string? Description { get; set; }

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }

    public int ContainerId { get; set; }

    public virtual Container Container { get; set; } = null!;
}
