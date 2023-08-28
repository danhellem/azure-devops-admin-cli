using adoAdmin.Models;
using System;
using System.Collections.Generic;
using System.Net;

namespace adoAdmin.ViewModels
{ 
    public class PlanList
    {
        public int count { get; set; }
        public List<Plan> value { get; set; }
    }
}
