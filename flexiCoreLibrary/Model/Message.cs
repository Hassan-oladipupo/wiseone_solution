using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Message
    {
        public long ID { get; set; }
        public long FromStaffID { get; set; }
        public long ToStaffID { get; set; }
        public string MessageText { get; set; }
        public DateTime DateSent { get; set; }
    }
}
