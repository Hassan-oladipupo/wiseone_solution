using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ShiftDto
    {
        public long StaffShiftID { get; set; }
        public long StaffID { get; set; }
        public string Day { get; set; }
        public decimal From { get; set; }
        public string Date { get; set; }
        public decimal To { get; set; }        
        public long BreakTimeDuration { get; set; }
        public bool Configure { get; set; }
        public bool Done { get; set; }
        public long FolderTimeDuration { get; set; }       
        public bool HasSupervision { get; set; }
        public string Status { get; set; }
        public ClassRoomDto Room { get; set; }
        public decimal OverTime { get; set; }
        public string OverTimeData { get; set; }
        public string Reason { get; set; }
        public bool Absent { get; set; }
        public string DeclineReason { get; set; }
        public SignInOutDto ClockInOutData { get; set; }
    }
}
