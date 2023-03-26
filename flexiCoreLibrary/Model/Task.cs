using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Task
    {
        public Task()
        {
            this.TaskUpdates = new HashSet<TaskUpdate>();
            this.TaskStaffs = new HashSet<TaskStaff>();
        }

        public long ID { get; set; }        
        [MaxLength(50)]
        public string Subject { get; set; }        
        public string Details { get; set; }       
        [MaxLength(100)]
        public string CreatedBy { get; set; }
        [MaxLength(100)]
        public string LastUpdatedBy { get; set; }
        public DateTime DateofCompletion { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        [MaxLength(200)]
        public string TaskLeader { get; set; }
        [MaxLength(20)]
        public string Status { get; set; }
        [MaxLength(20)]
        public string Type { get; set; }

        public virtual ICollection<TaskUpdate> TaskUpdates { get; set; }
        public virtual ICollection<TaskStaff> TaskStaffs { get; set; }
    }
}
