using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class FinancialYearMonthDto
    {
        public FinancialYearMonthDto()
        {
            this.Days = new HashSet<FinancialYearMonthDayDto>();
        }

        public long ID { get; set; }        
        public string Label { get; set; }        
        public string Month { get; set; }
        public long Year { get; set; }
        public bool Exclude { get; set; }
        public long FinancialYearID { get; set; }        
        public FinancialYearDto FinancialYear { get; set; }
        public ICollection<FinancialYearMonthDayDto> Days { get; set; }
    }
}
