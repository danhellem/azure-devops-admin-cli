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

namespace adoProcess.Repos
{
    public static class Process
    {       
        public static List<ProcessInfo> GetProcesses(VssConnection connection)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                List<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessInfo> list = client.GetListOfProcessesAsync().Result;
                
                return list;            
            }
            catch (Exception)
            {
                return null;
            }          
        }

        public static List<ProcessWorkItemType> GetWorkItemTypes(VssConnection connection, System.Guid processId)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                List<ProcessWorkItemType> list = client.GetProcessWorkItemTypesAsync(processId).Result;

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static ProcessWorkItemTypeField GetField(VssConnection connection, Guid processId, string witRefName, string fieldRefName)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();          

            try
            {
                ProcessWorkItemTypeField item = client.GetWorkItemTypeFieldAsync(processId, witRefName, fieldRefName).Result;
         
                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
        }       
    }
}
