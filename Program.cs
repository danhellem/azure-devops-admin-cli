using adoAdmin.Helper;
using adoAdmin.Helper.ConsoleTable;
using adoAdmin.Models;
using adoAdmin.ViewModels;
using adoAdmin.Http;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace adoAdmin
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

            args = SetArgumentsFromConfig(args);

            string org, pat, process, project, refname, name, type, action;
            string witrefname, targetprocess;
            int days;
            bool _deleteOverRide = false;

            try
            {   
                CheckArguments(args, out org, out pat, out project, out refname, out name, out type, out action, out process, out witrefname, out targetprocess, out days);     
                
                Uri baseUri = new Uri(org);

                VssCredentials clientCredentials = new VssCredentials(new VssBasicCredential("username", pat));
                VssConnection vssConnection = new VssConnection(baseUri, clientCredentials);

                // close work item type
                if (action == "clonewit")                
                {                   
                    Console.WriteLine("Start Validation...");

                    bool val = CloneWitAndProcessValidation(vssConnection, process, targetprocess, witrefname);
                    
                    if (! val) return 0;
                }               

                // action out all fields
                if (action == "listallfields")
                {
                    List<WorkItemField2> fields = Repos.Fields.GetAllFields(vssConnection);

                    var table = new ConsoleTable("Name", "Reference Name", "Type");

                    foreach (WorkItemField field in fields)
                    {
                        table.AddRow(field.Name, field.ReferenceName, field.Type);
                    }

                    table.Write();
                    Console.WriteLine();

                    return 0;
                }

                // action out all fields
                if (action == "allpicklists")
                {
                    Console.Write("Loading all picklists and fields: ");

                    List<WorkItemField2> fields = Repos.Fields.GetAllFields(vssConnection);
                    List<PickListMetadata> picklists = Repos.Process.ListPicklists(vssConnection);
                   
                    Console.Write("Done");
                    Console.WriteLine("");

                    var table = new ConsoleTable("Name", "Id", "Type", "Fields");

                    foreach (PickListMetadata item in picklists)
                    {
                        string fieldName = string.Empty;
                        var field = fields.Where(x => x.IsPicklist == true && x.PicklistId == item.Id);                    
                        fieldName = field.Count() > 0 ? field.FirstOrDefault().Name : String.Empty;                        

                        table.AddRow(item.Name, item.Id, item.Type, fieldName).Write();                        
                    }

                    table.Write();
                    Console.WriteLine();

                    return 0;
                }

                // action out all fields
                if (action == "picklistswithnofield")
                {
                    Console.Write(" Loading all picklists and fields: ");

                    List<WorkItemField2> fields = Repos.Fields.GetAllFields(vssConnection);
                    List<PickListMetadata> picklists = Repos.Process.ListPicklists(vssConnection);

                    Console.WriteLine("Done");
                                                         
                    var table = new ConsoleTable("Name", "Id", "Type");

                    // look through all the picklists and lets see if they are attached to any fields
                    foreach (PickListMetadata item in picklists)
                    {
                        string fieldName = string.Empty;
                        var field = fields.Where(x => x.IsPicklist == true && x.PicklistId == item.Id);
                        
                        // found one
                        if (field.Count() == 0)
                        {
                            table.AddRow(item.Name, item.Id, item.Type);
                        }                        
                    }

                    // check to see if we have any results
                    if (table.Rows.Count == 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" No unused picklists found.");
                        return 0;
                    }

                    Console.WriteLine();
                    Console.WriteLine(" These picklists are not being used by any fields:");

                    table.Write();                 

                    // if we have some results, do we want to delete the picklists?
                    if (table.Rows.Count > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine(" Would you like to delete these unused picklists?");
                        Console.WriteLine();
                        Console.WriteLine(" Press 'Y' continue or 'N' to abort.");

                        // no i don't
                        if (Console.ReadKey().Key == ConsoleKey.N)
                        {
                            Console.WriteLine(" Delete aborted!");
                            return 0;
                        }

                        // yes i do
                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Console.Write(" Deleting unused picklists: ");

                            // loop throuh the table and delete a picklist one at a time
                            foreach (IList<object> row in table.Rows)
                            {
                                Repos.Process.DeletePicklist(vssConnection, row[1].ToString());
                            }

                            Console.Write("Done");
                            Console.WriteLine(" ");
                        }                       
                    }

                    return 0;
                }

                // get one field by refname and me all of the processes that field is in
                if (action == "getfield" && (! String.IsNullOrEmpty(refname)))
                {
                    List<ProcessInfo> processList = Repos.Process.GetProcesses(vssConnection);
                    List<ProcessWorkItemType> witList;                    

                    var table = new ConsoleTable("Process", "Work Item Type", "Field Name", "Field Reference Name");

                    foreach (var processInfo in processList)
                    {
                        witList = Repos.Process.GetWorkItemTypes(vssConnection, processInfo.TypeId);

                        foreach(var witItem in witList)
                        {
                            ProcessWorkItemTypeField witField = Repos.Process.GetField(vssConnection, processInfo.TypeId, witItem.ReferenceName, refname);

                            if (witField != null)
                            {
                                table.AddRow(processInfo.Name, witItem.Name, witField.Name, witField.ReferenceName);                             
                            }

                            witField = null;
                        }
                    }

                    table.Write();
                    Console.WriteLine();                                      
                }

                // list of projects and work item types the field is used in
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

                    table.Write();
                    Console.WriteLine();

                    field = null;
                    table = null;
                    witList = null;
                }

                // search for a specific field by refname to see if it exists
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

                // add new field to the organization
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

                // list of fields in a process
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
                               
                // action out all fields
                if (action == "emptyrecyclebin")
                {
                    Console.Write(" Loading deleted items: ");

                    List<WorkItemReference> workitems = Repos.RecycleBin.GetDeletedWorkItemsByWiql(vssConnection, project, days);

                    Console.WriteLine(workitems.Count);

                    if (workitems.Count < 1)
                    {
                        Console.WriteLine($" There are no items in the recycle bin that are ready to be destroyed (-{days} days).");
                        Console.WriteLine(" Completed.");
                        return 0;
                    }

                    Console.WriteLine(" ");
                    Console.WriteLine(" WARNING!", Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine(" You are about to permentantly destroy work items. This action cannot be undone.", Console.ForegroundColor = ConsoleColor.Red);
                    Console.WriteLine(" ");
                    Console.ResetColor();
                    Console.WriteLine(" Press any key to continue or 'N' to abort.");

                    if (Console.ReadKey().Key == ConsoleKey.N) return 0;

                    // use delete override of batch api is failing (edge case bug)
                    // this will delete one a time in the current batch
                    if (_deleteOverRide)
                    {
                        Console.WriteLine($" Running Delete Override (one at a time)...");

                        int i = 1;

                        // Loop over strings.
                        foreach (WorkItemReference workitem in workitems)
                        {
                            Console.WriteLine($"  [{i}] Destroying work item {workitem.Id}");
                            Repos.RecycleBin.DestroyWorkItem(vssConnection, workitem.Id);
                            i = i + 1;
                        }

                        Console.WriteLine(" ");
                        Console.WriteLine($" Completed set of {workitems.Count}");

                        return 0;
                    }

                    while (workitems.Count > 0)
                    {
                        int i = 0;
                        String[] ids = workitems.Count >= 200 ? new String[200] : new String[workitems.Count];

                        foreach (WorkItemReference workitem in workitems)
                        {
                            ids[i] = workitem.Id.ToString();
                            i = i + 1;
                        }

                        string val = String.Join(",", ids);                
                      
                        Console.Write(" Destroying items: ");

                        IDestroyWorkItemsResponse response = Repos.RecycleBin.DestroyWorkItems(pat, org, val);

                        if (!response.Success)
                        {
                            Console.WriteLine("Failed.");
                            Console.WriteLine(" ");
                            Console.WriteLine(response.StatusCode);
                            Console.WriteLine(response.Message);
                            break;
                        }

                        Console.WriteLine("Success.");
                        Console.WriteLine(" ");

                        Console.Write(" Waiting a moment...");
                        Thread.Sleep(2000);
                        Console.WriteLine("done.");
                        Console.WriteLine(" ");

                        Console.Write(" Loading more deleted items: ");

                        workitems = Repos.RecycleBin.GetDeletedWorkItemsByWiql(vssConnection, project, days);

                        Console.WriteLine(workitems.Count);                                              
                    }

                    //table.Write();
                    Console.WriteLine(" ");
                    Console.WriteLine(" Complete.");

                    return 0;
                }

                // list tags and tag usage
                if (action == "listemptytags")
                {
                    Console.Write(" Loading tags: ");

                    List<WorkItemTagDefinition> tags = Repos.Tags.GetAllTags(vssConnection, project);

                    Console.Write("Done");
                    Console.WriteLine("");

                    var table = new ConsoleTable("Name", "Id");

                    Console.Write(" Looping through each tag to find linked worked items. This may take a while...");
                  
                    foreach (WorkItemTagDefinition tag in tags)
                    {
                        List<WorkItemReference> list = Repos.Tags.FetchWorkItemByTag(vssConnection, project, tag.Name);

                        if (list.Count == 0) {
                            table.AddRow(tag.Name, tag.Id);
                        }                      
                    }

                    Console.Write("Done");
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine(" Tags that can be deleted");

                    if (table.Rows .Count > 0 ) { table.Write(); } else { Console.WriteLine(" No empty tags found"); }
                    
                    Console.WriteLine();
                }

                // Delete a tag 
                if (action == "deletetag")
                {
                    Console.Write("Deleting tag: ");

                    try
                    {                        
                        Repos.Tags.DeleteTag(vssConnection, project, name);

                        Console.WriteLine($"Tag: '" + name + "' deleted successfully.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to delete tag: '" + name + "'.");
                        if ( e.InnerException is null )
                        {
                            Console.WriteLine(e.Message);
                        }
                        else
                        {
                            Console.WriteLine(e.InnerException.Message);
                        }
                    }
                    Console.WriteLine();
                }

                if (action == "list-delete-plans")
                {
                    Console.Write(" Loading delivery plans: ");

                    ConsoleKeyInfo cki;
                    List<string> list = new List<string>();

                    IGetPlansResponse response = Repos.DeliveryPlans.GetPlans(pat, org, project);
                    List<Models.Plan> plans = response.Plans.value.OrderByDescending(x => x.lastAccessed).ToList();

                    Console.Write("Done");
                    Console.WriteLine("");

                    var table = new ConsoleTable("Name", "Description", "Created", "Modified", "Last Accessed");

                    foreach (Models.Plan plan in response.Plans.value)
                    {
                        if (plan.lastAccessed.HasValue && plan.lastAccessed.Value.AddDays(days) < DateTime.Today)
                        {
                            table.AddRow(plan.name, plan.description, plan.createdDate, plan.modifiedDate, plan.lastAccessed);
                            list.Add(plan.id);
                        }

                        // get list of plans that have never been accessed
                        // for those plans that existed before we added last accessed fields
                        if (!plan.lastAccessed.HasValue)
                        {
                            table.AddRow(plan.name, plan.description, plan.createdDate, plan.modifiedDate, "Never");
                            list.Add(plan.id);
                        }
                    }

                    if (table.Rows.Count == 0)
                    {
                        Console.WriteLine($" No delivery plans found that have not been accessed in the last {days} days.");
                    }
                    else
                    {
                        table.Write();

                        Console.WriteLine();
                        Console.Write($" Press <Delete> to delete these {list.Count} plans or press <Enter> to exit... ");
                        Console.WriteLine();

                        do
                        {
                            cki = Console.ReadKey();
                            if (cki.Key == ConsoleKey.Delete)
                            {
                                Console.WriteLine("");
                                Console.Write(" Deleting plans: ");
                                Console.WriteLine("");

                                foreach (string id in list)
                                {
                                    Console.Write($"  - {id}");
                                    Repos.DeliveryPlans.DeletePlan(vssConnection, project, id);
                                }

                                Console.WriteLine("");
                            }
                        } while (cki.Key != ConsoleKey.Enter);

                        Console.Write("Done");
                    }

                    table = null;
                    response = null;
                    plans = null;
                    list = null;
                }

                if (action == "mylimits")
                {
                    var table = new ConsoleTable("Project limit", "Status", "Recommendation");

                    if (!string.IsNullOrEmpty(project))
                    {
                        Console.Write("Loading usage data for project limits: ");

                        List<WorkItemReference> workitems = Repos.RecycleBin.GetDeletedWorkItemsByWiql(vssConnection, project);
                        
                        table.AddRow("Recycle Bin", $"{workitems.Count} deleted work items", workitems.Count > 19000 ? $"Empty recycle bin" : $" - ").Write();

                        workitems = null;

                        Console.Write("Done");
                        Console.WriteLine("");
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
         
        private static bool CloneWitAndProcessValidation(VssConnection vssConnection, string process, string targetProcess, string witRefName)
        {
            List<ProcessInfo> processList = Repos.Process.GetProcesses(vssConnection);

            ProcessInfo sourceProcessInfo = processList.Find(x => x.Name == process);
            ProcessInfo targetProcessInfo = processList.Find(x => x.Name == targetProcess);

            Console.Write("  Validating source process '{0}'...", process);

            if (sourceProcessInfo == null)
            {
                Console.Write("failed (process not found) \n");
                return false;
            }
            else
            {
                Console.Write("done \n");
            };

            Console.Write("  Validating target process '{0}'...", targetProcess);

            if (targetProcessInfo == null)
            {
                Console.Write("failed (process not found) \n");
                return false;
            }
            else
            {
                Console.Write("done \n");
            };

            Console.Write("  Validating work item type '{0}' exists in source process...", witRefName);

            if (Repos.Process.GetWorkItemType(vssConnection, sourceProcessInfo.TypeId, witRefName) == null)
            {
                Console.Write("failed (work item type not found) \n");
                return false;
            }
            else
            {
                Console.Write("done \n");
            }

            Console.Write("  Validating work item type {0} does not exist in target process...", witRefName);

            if (Repos.Process.GetWorkItemType(vssConnection, targetProcessInfo.TypeId, witRefName) != null)
            {
                Console.Write("failed (work item type found) \n");
                return false;
            }
            else
            {
                Console.Write("done \n");
            }

            return true;
        }

        private static void CheckArguments(string[] args, out string org, out string pat, out string project, out string refname, out string name, out string type, out string action, out string process, out string witrefname, out string targetprocess, out int days)
        {
            org = null;
            refname = null;
            name = null;
            type = null;
            action = null;
            project = null;
            pat = null;
            process = null;
            witrefname = null;
            targetprocess = null;
            days = 0;

            //Dictionary<string, string> argsMap = new Dictionary<string, string>();
            
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
                        case "witrefname":
                            witrefname = value;
                            break;
                        case "targetprocess":
                            targetprocess = value;
                            break;
                        case "days":
                            days = Convert.ToInt32(value);
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

            if (action == "clonewit")            {                

                if (process == null)
                {
                    throw new ArgumentException("Missing required argument 'process'");
                }

                if (witrefname == null)
                {
                    throw new ArgumentException("Missing required argument 'witrefname' for the work item type you want to clone");
                }

                if (targetprocess == null)
                {
                    throw new ArgumentException("Missing required argument 'targetprocess' for the process you want to clone the work item type into");
                }
            } 
            
            if (action == "emptyrecyclebin" && string.IsNullOrEmpty(project))
            {
                throw new ArgumentException("Missing required argument 'project'");
            }

            if (action == "listemptytags" && string.IsNullOrEmpty(project))
            {
                throw new ArgumentException("Missing required argument 'project'");
            }

            if (action == "deletetag")
            {
                if (string.IsNullOrEmpty(project))
                {
                    throw new ArgumentException("Missing required argument 'project'");
                }

               if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Missing required argument 'name'");
                }
            }

            if (action == "list-delete-plans" && days == 0)
            {
                throw new ArgumentException("Missing required argument 'days'");
            }

            if (action == "list-delete-plans" && string.IsNullOrEmpty(project))
            {
                throw new ArgumentException("Missing required argument 'project'");
            }
        }        

        private static string[] SetArgumentsFromConfig (string[] args)
        {
            var configHelper = new ConfigHelper();
            bool org = false;
            bool pat = false;
           
            foreach (var arg in args)
            {
                if (arg[0] == '/' && arg.IndexOf(':') > 1)
                {
                    string key = arg.Substring(1, arg.IndexOf(':') - 1);
                    string value = arg.Substring(arg.IndexOf(':') + 1);

                    switch (key)
                    {
                        case "org":
                            org = true;
                            break;
                        case "pat":
                            pat = true;
                            break;
                        default:
                            break;
                    }
                }
            }

            if (! org && !String.IsNullOrEmpty(configHelper.Organization))
            {
                Array.Resize(ref args, args.Length + 1);
                args[args.Length - 1] = "/org:" + configHelper.Organization;
            }

            if (!pat && !String.IsNullOrEmpty(configHelper.PersonalAccessToken))
            {
                Array.Resize(ref args, args.Length + 1);
                args[args.Length - 1] = "/pat:" + configHelper.PersonalAccessToken;
            }

            configHelper = null;

            return args;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("CLI to manage an inherited process in Azure DevOps. View Readme.MD in GitHub repo for full details.");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /org:{value}               azure devops organization name");
            Console.WriteLine("  /pat:{value}               personal access token");
            Console.WriteLine("");
            Console.WriteLine("  /action:                   listallfields, getfieldforprojects, addfield, searchfield, listfieldsforprocess, allpicklists, picklistswithnofield, clonewit");
            Console.WriteLine("  /refname:{value}           refname of field getting or adding");
            Console.WriteLine("  /name:{value}              field friendly name");
            Console.WriteLine("  /process:{value}           name of process");
            Console.WriteLine("  /type:{value}              type field creating"); 
            Console.WriteLine("  /targetprocess:{value}     target process for where you want to clone a wit into");
                      
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listemptytags /project:projectname");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listallfields");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:allpicklists");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:picklistswithnofield");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:getfield /refname:System.Title");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listfieldsforprocess /process:Agile");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listfieldsforprocess /process:Agile");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:clonewit /process:sourceprocess /witrefname:custom.ticket /targetprocess:targetprocess");

            Console.WriteLine("");
        }
    }
}
