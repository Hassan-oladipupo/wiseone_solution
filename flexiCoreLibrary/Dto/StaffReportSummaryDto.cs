using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffReportSummaryDto
    {
        public StaffReportSummaryDto()
        {
            ReportIDs = new List<long>();
        }

        public long StaffID { get; set; }
        public string StaffName { get; set; }        
        public int AbsentTimes { get; set; }
        public List<long> ReportIDs { get; set; }
    }
}
