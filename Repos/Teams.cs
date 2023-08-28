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
    public static class Teams
    {    
        public static List<WorkItemDeleteShallowReference> ListAll(VssConnection connection, string project)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemDeleteShallowReference> wits = workItemTrackingClient.GetDeletedWorkItemShallowReferencesAsync(project).Result;

            return wits;              
        }       
    }    
}
