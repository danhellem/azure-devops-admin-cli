using adoProcess.Helper;
using adoProcess.Helper.ConsoleTable;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adoProcess.WorkItemTracking
{
    public static class Fields
    {   
        public static void GetAllFields(VssConnection connection)
        {            
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<WorkItemField> fields = workItemTrackingClient.GetFieldsAsync().Result;

            var table = new ConsoleTable("Name", "Reference Name", "Type");       

            foreach (WorkItemField field in fields)
            {
                table.AddRow(field.Name, field.ReferenceName, field.Type);        
            }

            table.Write();
            Console.WriteLine();

            fields = null;
            workItemTrackingClient = null;          
        }

        public static void GetField(VssConnection connection, string refname)
        {
            WorkItemTrackingHttpClient workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            try
            {
                WorkItemField field = workItemTrackingClient.GetFieldAsync(refname).Result;

                var table = new ConsoleTable("Name", "Reference Name", "Type");
                table.AddRow(field.Name, field.ReferenceName, field.Type);

                table.Write();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Field '" + refname + "' not found");
            }          
           
            workItemTrackingClient = null;
        }
    }
}
