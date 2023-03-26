using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ClassRoomDto
    {
        public long ID { get; set; }
        public string Name { get; set; }        
        public LocationDto Location { get; set; }
        public string Status { get; set; }
        public string CreatedOn { get; set; }
        public string LastModifiedOn { get; set; }
    }
}
