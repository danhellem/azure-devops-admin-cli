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

            string org, pat, project, refname, name, type, list;

            try
            {
                CheckArguments(args, out org, out pat, out project, out refname, out name, out type, out list);

                Uri baseUri = new Uri(org);

                VssCredentials clientCredentials = new VssCredentials(new VssBasicCredential("username", pat));
                VssConnection vssConnection = new VssConnection(baseUri, clientCredentials);

                if (list == "all" && String.IsNullOrEmpty(refname))
                {
                    WorkItemTracking.Fields.GetAllFields(vssConnection);

                    return 0;
                }

                if (list == "one" && (! String.IsNullOrEmpty(refname)))
                {
                    WorkItemTracking.Fields.GetField(vssConnection, refname);

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

        private static void CheckArguments(string[] args, out string org, out string pat, out string project, out string refname, out string name, out string type, out string list)
        {
            org = null;
            refname = null;
            name = null;
            type = null;
            list = null;
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
                        case "list":
                            list = value;
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

            if ((list == null) && (refname != null || name != null || type != null))
            {
                throw new ArgumentException("List only works with connectionurl argument");
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("CLI to manage an inherited process in Azure DevOps");
            Console.WriteLine("");
            Console.WriteLine("Arguments:");
            Console.WriteLine("");
            Console.WriteLine("  /org:{value}                                       URL of the account/collection to run the samples against.");
            Console.WriteLine("  /pat:{value}                                       personal access token");
            Console.WriteLine("");
            Console.WriteLine("  /refname:{value} /name:{value} /type:{value}       creates a new field");           
            Console.WriteLine("  /list:all                                          print out all fields");
            Console.WriteLine("  /list:one  /refname:{value}                        print field information for refname");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("");
            Console.WriteLine("  /org:fabrikam /pat:{value} /list:all");

            Console.WriteLine("");
        }
    }
}
