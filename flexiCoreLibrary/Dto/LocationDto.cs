using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class LocationDto
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public bool HeadOffice { get; set; }
        public bool OperationOffice { get; set; }
        public string CreatedOn { get; set; }
        public string LastUpdatedOn { get; set; }
    }
}
