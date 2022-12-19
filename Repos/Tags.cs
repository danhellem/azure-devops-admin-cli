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
using System.IO;

namespace adoAdmin.Repos
{
    public static class Tags
    {    
        public static List<WorkItemTagDefinition> GetAllTags(VssConnection connection, string project)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemTagDefinition> tags = workItemTrackingClient.GetTagsAsync(project).Result;

            return tags;              
        }

        public static List<WorkItemReference> FetchWorkItemByTag(VssConnection connection, string project, string tag)
        {
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            // if we don't use days, then set wiql accordingly
            string wiqlString = $"Select [System.Id] From WorkItems Where [System.TeamProject] = '{project}' AND [System.Tags] CONTAINS '{tag}'";

            Wiql wiql = new Wiql() { Query = wiqlString };
            WorkItemQueryResult wits = workItemTrackingClient.QueryByWiqlAsync(wiql, false, 200).Result;

            return wits.WorkItems.ToList();
        }
    }    
}

