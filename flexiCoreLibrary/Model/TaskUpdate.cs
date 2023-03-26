using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class TaskUpdate
    {
        public long ID { get; set; }
        public long TaskID { get; set; }        
        public string Update { get; set; }
        [MaxLength(200)]
        public string ReportedBy { get; set; }
        public DateTime ReportedOn { get; set; }           
        [MaxLength(20)]
        public string Status { get; set; }

        [ForeignKey("TaskID")]
        public virtual Task Task { get; set; }
    }
}
