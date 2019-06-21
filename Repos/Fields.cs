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
    public static class Fields
    {   
        private readonly static string[] _fieldTypes = new string[] { "boolean", "dateTime", "double", "html", "identity", "integer", "pickactionDouble", "pickactionInteger", "pickactionString", "plainText", "string" };

        public static List<WorkItemField> GetAllFields(VssConnection connection)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> fields = workItemTrackingClient.GetFieldsAsync().Result;

            return fields;               
        }

        public static List<WorkItemField> SearchFields(VssConnection connection, string name = null, string type = null)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> fields = workItemTrackingClient.GetFieldsAsync().Result;
            List<WorkItemField> results = new List<WorkItemField>();

            if (! string.IsNullOrEmpty(name))
            {
                foreach(var field in fields)
                {
                    if (field.Name.ToLower().Contains(name.ToLower()))
                    {
                        results.Add(field);
                    }
                }
            }  
            
            if (! string.IsNullOrEmpty(type))
            {
                var fieldType = SetFieldType(type);

                foreach (var field in fields)
                {
                    if (field.Type == fieldType)
                    {
                        results.Add(field);
                    }
                }
            }  

            return results;           
        }

        public static WorkItemField GetField(VssConnection connection, string refname)
        {
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                WorkItemField field = client.GetFieldAsync(refname).Result;
                
                return field;               
            }
            catch (Exception)
            {
                return null;
            }          
        }
        
        public static WorkItemTypeFieldWithReferences GetFieldForWorkItemType(VssConnection connection,string project, string workItemType, string fieldRefName)
        {
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            WorkItemTypeFieldWithReferences field = workItemTrackingClient.GetWorkItemTypeFieldWithReferencesAsync(project, workItemType, fieldRefName).Result;

            return field; 
        }

        public static WorkItemField AddField(VssConnection connection, string refname, string name, string type)
        {
            WorkItemTrackingHttpClient client = connection.GetClient<WorkItemTrackingHttpClient>();
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType fieldType = SetFieldType(type);

            WorkItemField workItemField = new WorkItemField()
            {
                Name = name,
                ReferenceName = refname,
                Type = fieldType
            };

            var result = client.CreateFieldAsync(workItemField).Result;

            return result;
        }

        public static List<FieldsPerProcess> ListFieldsForProcess(VssConnection connection, string process)
        {
            WorkItemTrackingProcessHttpClient client = connection.GetClient<WorkItemTrackingProcessHttpClient>();
            List<FieldsPerProcess> list = new List<FieldsPerProcess>();

            //get the process by name
            List<ProcessInfo> listProcess = client.GetListOfProcessesAsync().Result;

            ProcessInfo processInfo = listProcess.Find(x => x.Name == process);

            //if processInfo is null then just return null
            if (processInfo == null)
            {
                return null;
            }

            //get list of work item types per the processid           
            List<ProcessWorkItemType> listWorkItemTypes = client.GetProcessWorkItemTypesAsync(processInfo.TypeId).Result;
           
            //loop thru each wit and get the list of fields
            //add to viewmodel object and return that
            foreach(ProcessWorkItemType wit in listWorkItemTypes)
            {
                List<ProcessWorkItemTypeField> listFields = client.GetAllWorkItemTypeFieldsAsync(processInfo.TypeId, wit.ReferenceName).Result;
                
                if (listFields.Count > 0) 
                {
                    list.Add(new FieldsPerProcess() { workItemType = wit, fields = listFields });
                }
                else
                {
                    list.Add(null);
                }
            }

            return list;
        }

        public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType SetFieldType(string type)
        {
            Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType fieldType;

            switch (type)
            {
                case "string":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.String;
                    break;
                case "boolean":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Boolean;
                    break;
                case "dateTime":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.DateTime;
                    break;
                case "double":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Double;
                    break;
                case "html":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Html;
                    break;      
                case "identity":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Identity;
                    break;
                case "integer":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.Integer;
                    break;               
                case "plainText":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PlainText;
                    break;
                case "pickactionDouble":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistDouble;
                    break;
                case "pickactionInteger":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistInteger;
                    break;
               case "pickactionString":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistString;
                    break;
                default:
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.String;
                    break;            
            }       

            return fieldType;
        }

        public static string[] Types
        {
            get {  return _fieldTypes; }
        }
    }
}
