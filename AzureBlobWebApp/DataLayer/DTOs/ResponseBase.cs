using System.Net;

namespace AzureBlobWebApp.DataLayer.DTOs
{
    public class ResponseBase
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string StatusMessage { get; set; } = string.Empty;
    }
}
