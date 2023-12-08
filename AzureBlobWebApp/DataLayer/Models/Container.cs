namespace AzureBlobWebApp.DataLayer.Models;

public partial class Container
{
    public int ContainerId { get; set; }

    public string ContainerName { get; set; } = null!;

    public DateTime LastModified { get; set; }

    public int? ModifiedUserId { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual User User { get; set; } = null!;
}
