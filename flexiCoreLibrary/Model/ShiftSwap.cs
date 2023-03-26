using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class ShiftSwap
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
        public long RoomID { get; set; }

        public DateTime RequestedOn { get; set; }
        public string Status { get; set; }
        public string DeclineReason { get; set; }
        public decimal OverTime { get; set; }
    }
}
