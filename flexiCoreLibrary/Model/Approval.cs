using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Approval
    {
        public long ID { get; set; }
        public string Type { get; set; }
        public string Obj { get; set; }
        public Nullable<System.DateTime> RequestedOn { get; set; }
        public Nullable<System.DateTime> ApprovedOn { get; set; }
        public string Status { get; set; }
        public string DeclineReason { get; set; }
    }
}
