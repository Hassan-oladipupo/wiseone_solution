using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class TaskUpdateDto
    {
        public long ID { get; set; }        
        public string Update { get; set; }        
        public string ReportedBy { get; set; }
        public string ReportedOn { get; set; }        
        public string Status { get; set; }        
        public TaskDto Task { get; set; }
    }
}
