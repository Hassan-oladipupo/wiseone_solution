using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace flexiCoreLibrary.Model
{
    public class ApproverEmail
    {
        public long ID { get; set; }
        [MaxLength(50)]
        public string ApprovalType { get; set; }
        public long RequestingRoleID { get; set; }
        //serialized ApprovingRolesDto
        public string ApprovingRoles { get; set; }
    }
}
