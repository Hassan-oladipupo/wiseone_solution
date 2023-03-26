using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffLeaveDetailsDto
    {
        public List<StaffLeaveDto> StaffLeave { get; set; }
        public List<string> BankHolidays { get; set; }
        public decimal NumberOfLeaveTaken { get; set; }
        public decimal NumberOfPendingLeaveTaken { get; set; }
        public decimal NumberOfNonLeaveTaken { get; set; }
        public decimal NumberOfLeaveRemaining { get; set; }
        public decimal NumberOfLeaveDays { get; set; }
        public string FinancialYear { get; set; }
    }
}
