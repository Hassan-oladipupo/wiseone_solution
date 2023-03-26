using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class LeaveRequestDto
    {       
        public List<FinancialYearMonthDayDto> RequestedDays { get; set; }
        public long StaffID { get; set; }
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffTelephone { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }
        public decimal LeaveDaysTaken { get; set; }
        public decimal CurrentLeaveDaysTaken { get; set; }
        public decimal LeaveDaysRemaining { get; set; }
        public LocationDto StaffLocation { get; set; }
        public string LeaveDayDescription { get; set; }
        public long LeaveDaysAvailable { get; set; }
        public long BankHolidays { get; set; }
        public string LeaveType { get; set; }
        public string LeaveTypeDescription { get; set; }
        public bool LeaveIsDeductible { get; set; }
        public string DeclineReason { get; set; }
        public FinancialYearDto FinancialYear { get; set; }
    }
}
