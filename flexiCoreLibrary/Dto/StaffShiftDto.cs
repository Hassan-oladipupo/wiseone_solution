using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffShiftDto
    {
        public long ID { get; set; }
        public long StaffID { get; set; }
        public string StaffUsername { get; set; }
        public string StaffEmail { get; set; }
        public string StaffTelephone { get; set; }
        public string StaffName { get; set; }
        public string StaffKnownAs { get; set; }
        public string GeneralInformation { get; set; }
        public string WeekName { get; set; }
        public long ShiftConfigID { get; set; }
        public List<ShiftDto> Shift { get; set; }
    }
}
