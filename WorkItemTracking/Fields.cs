using adoProcess.Helper;
using adoProcess.Helper.ConsoleTable;
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
    public static class Fields
    {   
        private readonly static string[] _fieldTypes = new string[] { "boolean", "dateTime", "double", "html", "identity", "integer", "picklistDouble", "picklistInteger", "picklistString", "plainText", "string" };

        public static List<WorkItemField> GetAllFields(VssConnection connection)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> fields = workItemTrackingClient.GetFieldsAsync().Result;

            return fields;               
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
                case "picklistDouble":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistDouble;
                    break;
                case "picklistInteger":
                    fieldType = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.FieldType.PicklistInteger;
                    break;
               case "picklistString":
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
