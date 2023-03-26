using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class ShiftSwapRequest
    {
        public long ID { get; set; }
        public string FromShift { get; set; }
        public string ToShift { get; set; }
        public long FromStaffID { get; set; }
        public long ToStaffID { get; set; }
        public long LocationID { get; set; }
        public string Status { get; set; }
        public string DeclineReason { get; set; }
        public DateTime DateRequested { get; set; }
    }
}
