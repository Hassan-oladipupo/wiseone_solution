using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class SignInOutDto : ApprovalDto
    {
        public long ID { get; set; }
        public long StaffShiftID { get; set; }
        public StaffDto Staff { get; set; }
        public string StaffName { get; set; }
        public string Day { get; set; }
        public decimal From { get; set; }
        public string Date { get; set; }
        public decimal To { get; set; }
        public long BreakTimeDuration { get; set; }
        public long FolderTimeDuration { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public ClassRoomDto Room { get; set; }
        public bool ClockedIn { get; set; }
        public string ClockedInTime { get; set; }
        public bool ClockedOut { get; set; }
        public string ClockedOutTime { get; set; }
        //public bool StartedBreak { get; set; }
        //public string StartedBreakTime { get; set; }
        //public bool FinishedBreak { get; set; }
        //public string FinishedBreakTime { get; set; }
        public string SignInOutDate { get; set; }
        public decimal OverTime { get; set; }
        public string Reason { get; set; }
        public bool Absent { get; set; }
        public string DeclineReason { get; set; }
        //public bool SubmitOverTime { get; set; }
        //public decimal OverTimeSpent { get; set; }        
        //public string OverTimeApprovalStatus { get; set; }
    }
}
