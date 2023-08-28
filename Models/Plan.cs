using System;
using System.Net;

namespace adoAdmin.Models
{
    public class Plan
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? lastAccessed { get; set; }
        public DateTime? modifiedDate { get; set; }
    }
}
