using System.Net;

namespace adoAdmin.ViewModels
{
    public class DestroyWorkItemsResponse : IDestroyWorkItemsResponse
    {
        public bool Success { get; set; } = false;
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }

    public interface IDestroyWorkItemsResponse
    {
        bool Success { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string Message { get; set; }
    }
}
