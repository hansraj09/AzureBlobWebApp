namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class CTokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
