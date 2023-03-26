using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class LeaveApprovalDto
    {
        public LeaveApprovalDto()
        {
            this.LeaveRequest = new LeaveRequestDto();
            this.Approval = new ApprovalDto();
            this.FinancialYear = new FinancialYearDto();
        }

        public LeaveRequestDto LeaveRequest { get; set; }
        public ApprovalDto Approval { get; set; }
        public FinancialYearDto FinancialYear { get; set; }
    }
}
