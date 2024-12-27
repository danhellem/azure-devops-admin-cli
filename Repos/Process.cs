﻿using adoAdmin.Helper;
using adoAdmin.Helper.ConsoleTable;
using adoAdmin.ViewModels;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ActivityStatistic;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace adoAdmin.Repos
{
    public static class Process
    {       
        public static List<ProcessInfo> GetProcesses(VssConnection connection)
        {
            return GetProcesses(connection, false);
        }

        public static List<ProcessInfo> GetProcesses(VssConnection connection, bool ignoreSystem)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                List<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessInfo> list = client.GetListOfProcessesAsync().Result;

                if (ignoreSystem) 
                {
                    list.RemoveAll(x => x.Name == "CMMI" || x.Name == "Agile" || x.Name == "Scrum" || x.Name == "Basic");
                }

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
        
        public static ProcessWorkItemType GetWorkItemType(VssConnection connection, System.Guid processId, string witRefName)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                ProcessWorkItemType result = client.GetProcessWorkItemTypeAsync(processId, witRefName).Result;                

                return result;
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

        public static ProcessWorkItemType CloneWorkItemType(VssConnection connection, string witRefName, Guid processId)
        {           
            ProcessWorkItemType wit = Process.GetWorkItemType(connection, processId, witRefName);
            
            if (wit == null) return null;              
            
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            
            CreateProcessWorkItemTypeRequest createWitRequest = new CreateProcessWorkItemTypeRequest()
            {                
                Color = wit.Color,
                Description = wit.Description,
                Name = wit.Name,
                Icon = wit.Icon,
                InheritsFrom = wit.Inherits,
                IsDisabled = false                 
            };

            ProcessWorkItemType results = client.CreateProcessWorkItemTypeAsync(createWitRequest, processId).Result;

            return results;
        }

        public static List<PickListMetadata> ListPicklists(VssConnection connection)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                List<PickListMetadata> list = client.GetListsMetadataAsync(connection).Result;

                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Boolean DeletePicklist(VssConnection connection, string id)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();

            try
            {
                Guid listId = new Guid(id);

                client.DeleteListAsync(listId).SyncResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
