using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class TaskStaffDto
    {
        public long ID { get; set; }          
        public StaffDto Staff { get; set; }        
        public TaskDto Task { get; set; }
        public bool TaskLeader { get; set; }
    }
}
