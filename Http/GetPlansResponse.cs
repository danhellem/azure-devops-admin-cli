using adoAdmin.ViewModels;
using System.Net;

namespace adoAdmin.Http
{
    public class GetPlansResponse : IGetPlansResponse
    {
        public bool Success { get; set; } = false;
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public PlanList Plans { get; set;}
    }

    public interface IGetPlansResponse
    {
        bool Success { get; set; }
        HttpStatusCode StatusCode { get; set; }
        string Message { get; set; }
        PlanList Plans { get; set; }
    }
}
