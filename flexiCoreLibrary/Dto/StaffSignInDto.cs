using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffSignInDto
    {
        public StaffDto Staff { get; set; }
        public string ExpectedTotalHours { get; set; }
        public string ActualTotalHours { get; set; }
        public string OverTimeTotalHours { get; set; }
    }
}
