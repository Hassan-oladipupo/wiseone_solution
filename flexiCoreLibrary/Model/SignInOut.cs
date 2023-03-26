using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class SignInOut
    {
        public long ID { get; set; }
        public long StaffShiftID { get; set; }
        public long StaffID { get; set; }
        [MaxLength(30)]
        public string Day { get; set; }
        public decimal From { get; set; }
        [MaxLength(30)]
        public string Date { get; set; }
        public decimal To { get; set; }
        public long BreakTimeDuration { get; set; }       
        public long FolderTimeDuration { get; set; }        
        public long RoomID { get; set; }
        public bool ClockedIn { get; set; }
        public DateTime? ClockedInTime { get; set; }
        public bool ClockedOut { get; set; }
        public DateTime? ClockedOutTime { get; set; }
        //public bool StartedBreak { get; set; }
        //public DateTime? StartedBreakTime { get; set; }
        //public bool FinishedBreak { get; set; }
        //public DateTime? FinishedBreakTime { get; set; }
        public DateTime SignInOutDate { get; set; }
        public decimal OverTime { get; set; }
        public string AbsentReason { get; set; }
        public bool Absent { get; set; }
        public decimal LocationLat { get; set; }
        public decimal LocationLong { get; set; }
        public decimal ClockInLat { get; set; }
        public decimal ClockInLong { get; set; }
        public decimal ClockOutLat { get; set; }
        public decimal ClockOutLong { get; set; }

        [ForeignKey("RoomID")]        
        public virtual ClassRoom Room { get; set; }
        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }
        [ForeignKey("StaffShiftID")]
        public virtual StaffShift StaffShift { get; set; }
    }
}
