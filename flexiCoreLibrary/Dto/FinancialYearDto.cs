using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class FinancialYearDto
    {
        public FinancialYearDto()
        {
            this.Months = new HashSet<FinancialYearMonthDto>();
        }
        public long ID { get; set; }
        public LocationDto Location { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long LeavePriorNotice { get; set; }
        public List<ExcludeMonthDto> ExcludeMonths { get; set; }
        public string ExcludeMonthsDetails { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public ICollection<FinancialYearMonthDto> Months { get; set; }
        public List<LeaveTypeDto> LeaveTypes { get; set; }

        public string Label
        {
            get
            {
                return $"{StartDate} - {EndDate}";
            }
        }

    }
}
