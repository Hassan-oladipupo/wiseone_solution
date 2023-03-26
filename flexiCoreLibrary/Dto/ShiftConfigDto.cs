using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ShiftConfigDto
    {
        public ShiftConfigDto()
        {
            this.StaffShifts = new List<StaffShiftDto>();
        }

        public long ID { get; set; }
        public DateUtil StartDate { get; set; }
        public DateUtil EndDate { get; set; }
        public string StartDateStr { get; set; }
        public string EndDateStr { get; set; }
        public string WeekName { get; set; }
        public string GeneralInformation { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastUpdatedOn { get; set; }
        public LocationDto Location { get; set; }
        public virtual List<StaffShiftDto> StaffShifts { get; set; }
    }
}
