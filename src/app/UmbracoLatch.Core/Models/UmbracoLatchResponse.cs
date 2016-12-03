namespace UmbracoLatch.Core.Models
{
    public class UmbracoLatchResponse
    {

        public UmbracoLatchResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
    }
}