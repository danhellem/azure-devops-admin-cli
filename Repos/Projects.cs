using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

using System.Collections.Generic;
using System.Linq;

namespace adoAdmin.Repos
{
    public static class Projects
    {    
        public static List<TeamProjectReference> GetAllProjects(VssConnection connection)
        {            
            ProjectHttpClient client = connection.GetClient<ProjectHttpClient>();

            IPagedList<TeamProjectReference> projects = client.GetProjects(ProjectState.WellFormed).Result;
          
            return projects.ToList();
        }
    }    
}
