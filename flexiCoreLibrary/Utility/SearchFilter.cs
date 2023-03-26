using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary
{
    public class SearchFilter
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public string StaffName { get; set; }
        public string Username { get; set; }
        public bool CanApprove { get; set; }
        public DateUtil FromDate { get; set; }
        public DateUtil ToDate { get; set; }
        public long LocationID { get; set; }
        public long StaffID { get; set; }
    }
}
