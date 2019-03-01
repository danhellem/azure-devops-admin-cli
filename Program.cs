using adoProcess.Helper.ConsoleTable;
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

            string org, pat, project, refname, name, type, action;

            try
            {
                CheckArguments(args, out org, out pat, out project, out refname, out name, out type, out action);

                Uri baseUri = new Uri(org);

                VssCredentials clientCredentials = new VssCredentials(new VssBasicCredential("username", pat));
                VssConnection vssConnection = new VssConnection(baseUri, clientCredentials);

                //action out all fields
                if (action == "listallfields")
                {
                    var fields = WorkItemTracking.Fields.GetAllFields(vssConnection);

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
                    var field = WorkItemTracking.Fields.GetField(vssConnection, refname);

                    if (field != null)
                    {
                        var table = new ConsoleTable("Name", "Reference Name", "Type");

                        table.AddRow(field.Name, field.ReferenceName, field.Type);

                        table.Write();
                        Console.WriteLine();

                        return 0;
                    }
                    else
                    {
                        Console.WriteLine("Field '" + refname + "' not found");
                        return 0;
                    }                   
                }

                if (action == "searchfields")
                {
                    var fields = WorkItemTracking.Fields.SearchFields(vssConnection, name, type);

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
                    int pos = Array.IndexOf(WorkItemTracking.Fields.Types, type);

                    if (pos == -1)
                    {
                        var types = WorkItemTracking.Fields.Types;

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
                    var field = WorkItemTracking.Fields.GetField(vssConnection, refname);

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
                                       
                    WorkItemField newField = WorkItemTracking.Fields.AddField(vssConnection, refname, name, type);

                    if (newField != null)
                    {
                        Console.WriteLine("Field '" + refname + "' was successfully added");
                    }
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

        private static void CheckArguments(string[] args, out string org, out string pat, out string project, out string refname, out string name, out string type, out string action)
        {
            org = null;
            refname = null;
            name = null;
            type = null;
            action = null;
            project = null;
            pat = null;

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

            if ((action == "addfield") && (string.IsNullOrEmpty(refname) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type)))
            {
                throw new ArgumentException("addfield action requires refname, name, and type value");
            }

            if ((action == "searchfield" && string.IsNullOrEmpty(name) && string.IsNullOrEmpty(name)))
            {
                 throw new ArgumentException("searchfield action requires name or type value");
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
            Console.WriteLine("  /action:               listallfields, getfield, addfield");
            Console.WriteLine("  /refname:{value}       refname of field getting or adding");
            Console.WriteLine("  /name:{value}          field friendly name");
            Console.WriteLine("  /type:{value}          type field creating");             
          
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:listallfields");
            Console.WriteLine("  /org:fabrikam /pat:{value} /action:getfield /refname:System.Title");

            Console.WriteLine("");
        }
    }
}
