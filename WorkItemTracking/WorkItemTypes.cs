using adoProcess.Helper;
using adoProcess.Helper.ConsoleTable;
using adoProcess.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace adoProcess.WorkItemTracking
{
    public static class WorkItemTypes
    {    
        public static List<WorkItemType> GetWorkItemTypesForProject(VssConnection connection, string project)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemType> wits = workItemTrackingClient.GetWorkItemTypesAsync(project).Result;

            return wits;              
        }
    }    
}
