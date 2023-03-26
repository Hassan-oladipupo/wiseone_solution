using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class TaskDto
    {
        public TaskDto()
        {
            this.TaskUpdates = new HashSet<TaskUpdateDto>();
            this.TaskStaffs = new HashSet<TaskStaffDto>();
        }

        public long ID { get; set; }        
        public string Subject { get; set; }
        public string Details { get; set; }        
        public string CreatedBy { get; set; }
        public string DateofCompletionStr { get; set; }
        public DateUtil DateofCompletion { get; set; }
        public string LastUpdatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string LastUpdatedOn { get; set; }
        public string TaskLeader { get; set; }        
        public string Status { get; set; }
        public string Type { get; set; }

        public ICollection<TaskUpdateDto> TaskUpdates { get; set; }
        public ICollection<TaskStaffDto> TaskStaffs { get; set; }
    }
}
