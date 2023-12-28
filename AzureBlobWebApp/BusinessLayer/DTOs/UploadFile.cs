namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class UploadFile
    {
        public IFormFile file { get; set; }
        public string description { get; set; }
    }
}
