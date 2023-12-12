using System.Net;
using AzureBlobWebApp.DataLayer.DTOs;

namespace AzureBlobWebApp.BusinessLayer.DTOs
{
    public class BlobResponse: ResponseBase
    {
        public BlobResponse()
        {
            Blob = new Blob();
        }
        public Blob Blob { get; set; }
    }
}
