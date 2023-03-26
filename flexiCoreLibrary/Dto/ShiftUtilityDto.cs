using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ShiftUtilityDto
    {
        public ShiftUtilityDto()
        {
            this.Rooms = new List<ClassRoomDto>();
            this.Staffs = new List<StaffDto>();
            this.Locations = new List<LocationDto>();
        }

        public List<ClassRoomDto> Rooms { get; set; }
        public List<StaffDto> Staffs { get; set; }
        public List<LocationDto> Locations { get; set; }
    }
}
