namespace AzureBlobWebApp.DataLayer.DTOs
{
    public class TokenResponse : ResponseBase
    {
        public string JWTToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
