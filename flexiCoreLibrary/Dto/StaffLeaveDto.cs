using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffLeaveDto
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
        public List<FinancialYearMonthDayDto> RequestedDays { get; set; }
        public long FinancialYearID { get; set; }
        public string FinancialYearStartDate { get; set; }
        public long FinancialYearEndDate { get; set; }
        public string RequestedOn { get; set; }
        public string ApprovedOn { get; set; }
        public string LeaveType { get; set; }
        public string LeaveTypeDescription { get; set; }
        public string LeaveCriteria{ get; set; }
        public bool LeaveIsDeductible { get; set; }
        public ApprovalDto Approval { get; set; }
    }
}
