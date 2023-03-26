using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class StaffLeave
    {
        public long ID { get; set; }
        public long StaffID { get; set; }
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }
        public long LocationID { get; set; }
        public string LocationName { get; set; }
        public decimal NumberOfLeaveDaysTaken { get; set; }
        public string RequestedDays { get; set; }
        public long FinancialYearID { get; set; }
        public string FinancialYearStartDate { get; set; }
        public string FinancialYearEndDate { get; set; }
        public string RequestedOn { get; set; }
        public string ApprovedOn { get; set; }
        public Enums.LeaveType LeaveType { get; set; }
        public string LeaveCriteria { get; set; }
    }
}
