using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace adoAdmin.ViewModels
{
    public class FieldsPerProcess
    {
        public ProcessWorkItemType workItemType { get; set;}
        public List<ProcessWorkItemTypeField> fields {  get; set; }
    }
}
