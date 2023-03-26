using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class ShiftConfig
    {
        public ShiftConfig()
        {
            this.StaffShifts = new HashSet<StaffShift>();            
        }

        public long ID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(50)]
        public string WeekName { get; set; }
        public string GeneralInformation { get; set; }
        [MaxLength(50)]
        public string CreatedBy { get; set; }               
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public long LocationID { get; set; }
               
        public virtual ICollection<StaffShift> StaffShifts { get; set; }        
    }
}
