using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class StaffShiftFeed
    {
        public long ID { get; set; }            
        public long StaffID { get; set; }
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffPhoneNumber { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }
        public long LocationID { get; set; }
        public string LocationName { get; set; }
        public string RoomName { get; set; }

        public long StaffShiftID { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public decimal From { get; set; }               
        public decimal To { get; set; }
        public long BreakTimeDuration { get; set; }                
        public bool ClockedIn { get; set; }
        public DateTime? ClockedInTime { get; set; }
        public bool ClockedOut { get; set; }
        public DateTime? ClockedOutTime { get; set; }
        public string AbsentReason { get; set; }
        public bool Absent { get; set; }
        public bool Reported { get; set; }

        public DateTime CreatedOn { get; set; }                    
    }
}
