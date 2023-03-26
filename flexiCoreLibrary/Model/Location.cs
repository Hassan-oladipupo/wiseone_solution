using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Location
    {
        public Location()
        {            
            this.ClassRooms = new HashSet<ClassRoom>();
            this.Staff = new HashSet<Staff>();
        }

        public long ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Address { get; set; }
        [MaxLength(50)]
        public string Status { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public bool HeadOffice { get; set; }
        public bool OperationOffice { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }

        public virtual ICollection<ClassRoom> ClassRooms { get; set; }        
        public virtual ICollection<Staff> Staff { get; set; }
    }
}
