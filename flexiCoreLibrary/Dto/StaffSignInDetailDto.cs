using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffSignInDetailDto
    {        
        public string Date { get; set; }
        public string ShiftStartTime { get; set; }
        public string ShiftEndTime { get; set; }
        public string ClockInTime { get; set; }
        //public decimal StartBreakTime { get; set; }
        //public decimal EndBreakTime { get; set; }
        public string ClockOutTime { get; set; }
        public string ApprovedOverTime { get; set; }
    }
}
