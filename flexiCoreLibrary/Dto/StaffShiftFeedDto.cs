using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffShiftFeedDto
    {
        public long ID { get; set; }        
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffPhoneNumber { get; set; }
        public string StaffPicture { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }        
        public string RoomName { get; set; }

        public long StaffShiftID { get; set; }
        public string Date { get; set; }
        public string Day { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public long BreakTimeDuration { get; set; }
        public bool ClockedIn { get; set; }
        public string ClockedInTime { get; set; }
        public bool ClockedOut { get; set; }
        public string ClockedOutTime { get; set; }        

        public string AbsentReason { get; set; }
        public bool Absent { get; set; }

        public string CreatedOn { get; set; }
    }
}
