using adoProcess.Helper.ConsoleTable;
using adoProcess.ViewModels;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adoProcess
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowUsage();
                return 0;
            }

            string org, pat, process, project, refname, name, type, action;

            try
            {
                CheckArguments(args, out org, out pat, out project, out refname, out name, out type, out action, out process);

                Uri baseUri = new Uri(org);

                VssCredentials clientCredentials = new VssCredentials(new VssBasicCredential("username", pat));
                VssConnection vssConnection = new VssConnection(baseUri, clientCredentials);

                //action out all fields
                if (action == "listallfields")
                {
                    var fields = Repos.Fields.GetAllFields(vssConnection);

                    var table = new ConsoleTable("Name", "Reference Name", "Type");

                    foreach (WorkItemField field in fields)
                    {
                        table.AddRow(field.Name, field.ReferenceName, field.Type);
                    }

                    table.Write();
                    Console.WriteLine();

                    return 0;
                }

                //get one field by refname
                if (action == "getfield" && (! String.IsNullOrEmpty(refname)))
                {

                    //List<ProcessInfo> processList = Process.Process.GetProcesses(vssConnection);
                    //List<ProcessWorkItemType> witList;                    

                    //var table = new ConsoleTable("Process", "Work Item Type", "Field Name", "Field Reference Name");

                    //foreach (var processInfo in processList)
                    //{
                    //    witList = Process.Process.GetWorkItemTypes(vssConnection, processInfo.TypeId);

                    //    foreach(var wit in witList)
                    //    {
                    //        ProcessWorkItemTypeField witField = Process.Process.GetField(vssConnection, processInfo.TypeId, wit.ReferenceName, refname);

                    //        if (witField != null)
                    //        {
                    //            table.AddRow(processInfo.Name, wit.Name, witField.Name, witField.ReferenceName);                             
                    //        }

                    //        witField = null;
                    //    }
                    //}

                    //table.Write();
                    //Console.WriteLine();                                      
                }

                if (action == "getfieldforprojects" && (!String.IsNullOrEmpty(refname)))
                {
                    Console.WriteLine("Getting list of projects and work item types...");
                    Console.WriteLine();

                    var table = new ConsoleTable("Project", "Work Item Type", "Field Reference Name", "Field Name");
                    WorkItemTypeFieldWithReferences field;

                    List<TeamProjectReference> projectList = Repos.Projects.GetAllProjects(vssConnection);
                    List<WorkItemType> witList = null;

                    foreach (TeamProjectReference projectItem in projectList)
                    {                        
                        witList = Repos.WorkItemTypes.GetWorkItemTypesForProject(vssConnection, projectItem.Name);

                        foreach (WorkItemType witItem in witList)
                        {
                            field = Repos.Fields.GetFieldForWorkItemType(vssConnection, projectItem.Name, witItem.ReferenceName, refname);

                            if (field != null)
                            {
                                table.AddRow(projectItem.Name, witItem.ReferenceName, field.ReferenceName, field.Name);
                            }

                            field = null;
                        }
                    }

                    //List<WorkItemType> witList = Repos.WorkItemTypes.GetWorkItemTypesForProject(vssConnection, project);

                    //foreach(WorkItemType item in witList)
                    //{
                    //    field = Repos.Fields.GetFieldForWorkItemType(vssConnection, project, item.ReferenceName, refname);

                    //    if (field != null)
                    //    {
                    //        table.AddRow(project, item.ReferenceName, field.ReferenceName, field.Name);                           
                    //    }

                    //    field = null;
                    //}

                    table.Write();
                    Console.WriteLine();

                    field = null;
                    table = null;
                    witList = null;
                }

                if (action == "searchfields")
                {
                    var fields = Repos.Fields.SearchFields(vssConnection, name, type);

                    if (fields.Count == 0)
                    {
                        Console.WriteLine("No fields found for name: '" + name + "' or type: '" + type + "'");
                        return 0;
                    }

                    var table = new ConsoleTable("Name", "Reference Name", "Type");

                    foreach (WorkItemField field in fields)
                    {
                        table.AddRow(field.Name, field.ReferenceName, field.Type);
                    }

                    table.Write();
                    Console.WriteLine();

                    return 0;
                }               

                //add new field to the organization
                if (action == "addfield")
                {
                    //check to see if the type is a legit type
                    int pos = Array.IndexOf(Repos.Fields.Types, type);

                    if (pos == -1)
                    {
                        var types = Repos.Fields.Types;

                        Console.WriteLine("Invalid field type value '" + type + "'");
                        Console.WriteLine();
                        Console.WriteLine("Valid field types are:");
                        Console.WriteLine();

                        foreach (string item in types)
                        {
                            Console.WriteLine(item);
                        }

                        return 0;
                    }

                    //check and make sure the field does not yet exist
                    var field = Repos.Fields.GetField(vssConnection, refname);

                    if (field != null)
                    {
                        Console.WriteLine("Field already exists");
                        Console.WriteLine();

                        var table = new ConsoleTable("Name", "Reference Name", "Type");

                        table.AddRow(field.Name, field.ReferenceName, field.Type);

                        table.Write();
                        Console.WriteLine();

                        return 0;
                    }
                                       
                    WorkItemField newField = Repos.Fields.AddField(vssConnection, refname, name, type);

                    if (newField != null)
                    {
                        Console.WriteLine("Field '" + refname + "' was successfully added");
                    }
                }

                if (action == "listfieldsforprocess")
                {
                    List<FieldsPerProcess> list = Repos.Fields.ListFieldsForProcess(vssConnection, process);

                    var table = new ConsoleTable("Work Item Type", "Name", "Reference Name", "Type");

                    foreach (FieldsPerProcess item in list)
                    {
                        List<ProcessWorkItemTypeField> fields = item.fields;

                        foreach(ProcessWorkItemTypeField field in fields)
                        {
                            table.AddRow(item.workItemType.Name, field.Name, field.ReferenceName, field.Type);
                        }                      
                    }

                    table.Write();
                    Console.WriteLine();

                    return 0;
                }

                vssConnection = null;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);

                ShowUsage();
                return -1;
            }

            return 0;
        }

        private static void CheckArguments(string[] args, out string org, out string pat, out string project, out string refname, out string name, out string type, out string action, out string process)
        {
            org = null;
            refname = null;
            name = null;
            type = null;
            action = null;
            project = null;
            pat = null;
            process = null;

            Dictionary<string, string> argsMap = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key)
                    {
                        case "org":
                            org = "https://dev.azure.com/" + value;
                            break;
                        case "pat":
                            pat = value;
                            break;
                        case "project":
                            project = value;
                            break;
                        case "refname":
                            refname = value;
                            break;
                        case "name":
                            name = value;
                            break;
                        case "type":
                            type = value;
                            break;
                        case "action":
                            action = value;
                            break;
                        case "process":
                            process = value;
                            break;
                        default:
                            throw new ArgumentException("Unknown argument", key);
                    }
                }
            }

            if (org == null || pat == null)
            {
                throw new ArgumentException("Missing required arguments");
            }
            
            if ((action == "getfield") && string.IsNullOrEmpty(refname))
            {
                throw new ArgumentException("getfield action requires refname value");
            }

            if ((action == "getfieldforprojects") && string.IsNullOrEmpty(refname))
            {
                throw new ArgumentException("getfieldforprojects action requires field refname value");
            }

            if ((action == "addfield") && (string.IsNullOrEmpty(refname) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)))
            {
                throw new ArgumentException("addfield action requires refname, name, and type value");
            }

            if ((action == "searchfield" && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(refname)))
            {
                 throw new ArgumentException("searchfield action requires name or type value");
            }    
            
            if (action == "listfieldsforprocess" && string.IsNullOrEmpty(process))
            {
                throw new ArgumentException("listfieldsforprocess action requires process");
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("CLI to manage an inherited process in Azure DevOps");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /org:{value}           azure devops organization name");
            Console.WriteLine("  /pat:{value}           personal access token");
            Console.WriteLine("");
            Console.WriteLine("  /action:               listallfields, getfieldforprojects, addfield, searchfield, listfieldsforprocess");
            Console.WriteLine("  /refname:{value}       refname of field getting or adding");
            Console.WriteLine("  /name:{value}          field friendly name");
            Console.WriteLine("  /process:{value}       name of process");
            Console.WriteLine("  /type:{value}          type field creating");             
          
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listallfields");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:getfield /refname:System.Title");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listfieldsforprocess /process:Agile");

            Console.WriteLine("");
        }
    }
}
