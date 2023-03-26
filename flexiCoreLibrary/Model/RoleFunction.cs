using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class RoleFunction
    {
        public long ID { get; set; }

        public long RoleID { get; set; }

        public long FunctionID { get; set; }

        [ForeignKey("FunctionID")]
        public virtual Function Function { get; set; }

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }
    }
}
