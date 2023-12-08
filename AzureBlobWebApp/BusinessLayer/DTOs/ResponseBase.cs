using System.Net;

namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class ResponseBase
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string StatusMessage { get; set; } = string.Empty;
    }
}
