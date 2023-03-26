using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class TaskStaff
    {
        public long ID { get; set; }
        public long TaskID { get; set; }
        public long StaffID { get; set; }
        public bool TaskLeader { get; set; }

        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }
        [ForeignKey("TaskID")]
        public virtual Task Task { get; set; }
    }
}
