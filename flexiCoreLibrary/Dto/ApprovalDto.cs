using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ApprovalDto
    {
        public long ApprovalID { get; set; }
        public string ApprovalStatus { get; set; }
        public string RequestedOn { get; set; }
        public string ApprovedOn { get; set; }
    }
}
