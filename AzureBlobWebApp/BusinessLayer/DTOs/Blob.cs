namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class Blob
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

        public string GUID { get; set; } = null!;
    }
}
