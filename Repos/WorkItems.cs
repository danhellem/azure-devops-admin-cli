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
    public static class WorkItems
    {    
        public static List<WorkItemDeleteShallowReference> GetDeletedWorkItems(VssConnection connection, string project)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteShallowReference> wits = workItemTrackingClient.GetDeletedWorkItemShallowReferencesAsync(project).Result;

            return wits;              
        }
       
        public static List<WorkItemReference> GetWorkItemsByWiql(VssConnection connection, string project)
        {
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // if we don't use days, then set wiql accordingly
            string wiqlString = 
                $"Select [System.Id] From WorkItems Where [System.TeamProject] = '{project}' AND ([System.WorkItemType] NOT IN GROUP 'Microsoft.TestPlanCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestSuiteCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.TestCaseCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedParameterCategory' AND [System.WorkItemType] NOT IN GROUP 'Microsoft.SharedStepCategory') ORDER BY [System.ChangedDate] ASC";
               
            Wiql wiql = new Wiql() { Query = wiqlString };
            WorkItemQueryResult wits = workItemTrackingClient.QueryByWiqlAsync(wiql, false, top: 19999).Result;

            return wits.WorkItems.ToList();
        }
    }    
}
