using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using adoAdmin.ViewModels;
using System.IO;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System;
using adoAdmin.Http;
using Newtonsoft.Json;

namespace adoAdmin.Repos
{
    public static class DeliveryPlans
    {    
        public static IGetPlansResponse GetPlans(string pat, string organizationUrl, string project)
        {
            IGetPlansResponse returnResponse = new GetPlansResponse();

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(org);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", pat))));                                
               
                HttpResponseMessage response = client.GetAsync($"{organizationUrl}/{project}/_apis/work/plans?api-version=7.1-preview.1").Result;

                string result = response.Content.ReadAsStringAsync().Result;

                returnResponse.StatusCode = response.StatusCode;
                returnResponse.Message = "Success";
                returnResponse.Success = response.StatusCode == System.Net.HttpStatusCode.NoContent ? true : false;
                returnResponse.Plans = JsonConvert.DeserializeObject<ViewModels.PlanList>(result);

                client.Dispose();

                return returnResponse;
            }
        }  
        
        public static void DeletePlan(VssConnection connection, string project, string id)
        {
            WorkHttpClient client = connection.GetClient<WorkHttpClient>();

            client.DeletePlanAsync(project, id).SyncResult();            
        }
    }    
}
