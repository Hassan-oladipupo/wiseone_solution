using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ShiftSwapDto : ApprovalDto
    {
        public long ID { get; set; }
        public long StaffShiftID { get; set; }
        public string StaffShiftWeek { get; set; }
        public long ShiftConfigID { get; set; }
        public long StaffID { get; set; }
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }
        public long LocationID { get; set; }

        public string Day { get; set; }
        public long From { get; set; }
        public string Date { get; set; }
        public long To { get; set; }
        public ClassRoomDto Room { get; set; }
        public decimal OverTime { get; set; }
        public string OverTimeData { get; set; }

        public string RequestedOn { get; set; }
        public string Status { get; set; }
    }
}
