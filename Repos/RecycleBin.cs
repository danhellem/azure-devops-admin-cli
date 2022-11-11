using adoAdmin.Helper;
using adoAdmin.Helper.ConsoleTable;
using adoAdmin.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ActivityStatistic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.Services.Organization.Client;

namespace adoAdmin.Repos
{
    public static class RecycleBin
    {    
        public static List<WorkItemDeleteShallowReference> GetDeletedWorkItems(VssConnection connection, string project)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteShallowReference> wits = workItemTrackingClient.GetDeletedWorkItemShallowReferencesAsync(project).Result;

            return wits;              
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="project">Name or project</param>
        /// <param name="days">Number of days to ignore from today. For example, use 365 if you don't want to get the work items that have been deleted in the last year. Use 0 if you want to get everything.</param>
        /// <returns></returns>
        public static List<WorkItemReference> GetDeletedWorkItemsByWiql(VssConnection connection, string project, int days = 0)
        {
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();
            
            // if we don't use days, then set wiql accordingly
            string wiqlString = days == 0 
                ? $"Select [System.Id] From WorkItems Where [System.TeamProject] = '{project}' AND [System.IsDeleted] = true AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory') ORDER BY [System.ChangedDate] ASC" 
                : $"Select [System.Id] From WorkItems Where [System.TeamProject] = '{project}' AND [System.IsDeleted] = true AND [System.ChangedDate] < @Today - {days} AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory') ORDER BY [System.ChangedDate] ASC";
            
            Wiql wiql = new Wiql() { Query = wiqlString };
            WorkItemQueryResult wits = workItemTrackingClient.QueryByWiqlAsync(wiql, false, 200).Result;

            return wits.WorkItems.ToList();
        }

        public static IDestroyWorkItemsResponse DestroyWorkItems(string pat, string organizationUrl, string ids) {

            IDestroyWorkItemsResponse returnResponse = new DestroyWorkItemsResponse();

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(org);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                    Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", pat))));

                string payload = "{\"Ids\": [" + ids + "],\"destroy\": true, \"skipNotifications\": true}";
                HttpContent body = new StringContent(payload, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync($"{organizationUrl}/_apis/wit/workitemsdelete?api-version=7.1-preview.1", body).Result;

                returnResponse.StatusCode = response.StatusCode;
                returnResponse.Message = response.Content.ReadAsStringAsync().Result.ToString();
                returnResponse.Success = response.StatusCode == System.Net.HttpStatusCode.NoContent ? true : false;                

                client.Dispose();

                return returnResponse;
            }
        }

    }    
}
