using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ReportDto
    {       
        public string From { get; set; }        
        public string To { get; set; }
        public string RoomName { get; set; }
        public string LocationName { get; set; }
        public string StaffName { get; set; }
        public string Reason { get; set; }
    }
}
