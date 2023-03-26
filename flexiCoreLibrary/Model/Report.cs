using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Report
    {
        public long ID { get; set; }
        public long StaffShiftID { get; set; }
        public long StaffID { get; set; }
        public string Day { get; set; }
        public decimal From { get; set; }
        public string Date { get; set; }
        public decimal To { get; set; }       
        public string RoomName { get; set; }
        public string LocationName { get; set; }
        public string StaffName { get; set; }
        public DateTime DateReported { get; set; }
        public string Reason { get; set; }
        public Enums.ReportType ReportType { get; set; }
        public bool MailSent { get; set; }
    }
}
