using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class MessageDto
    {
        public long FromStaffID { get; set; }
        public List<long> ToStaffIDs { get; set; }
        public string MessageText { get; set; }
    }
}
