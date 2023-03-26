using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ShiftSwapRequestDto : ApprovalDto
    {
        public long ID { get; set; }
        public ShiftSwapDto FromShift { get; set; }
        public ShiftSwapDto ToShift { get; set; }
        public long FromStaffID { get; set; }
        public long ToStaffID { get; set; }
        public long LocationID { get; set; }
        public bool ToMe { get; set; }
        public string Status { get; set; }
        public string DateRequested { get; set; }
        public string DeclineReason { get; set; }
    }
}
