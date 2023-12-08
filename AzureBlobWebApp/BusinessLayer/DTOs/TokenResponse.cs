namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class TokenResponse
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
