using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ApproverEmailDto
    {
        public long ID { get; set; }        
        public string ApprovalType { get; set; }
        public string ApprovalTypeDesc { get; set; }
        public RoleDto RequestingRole { get; set; }
        public List<ApprovingRolesDto> ApprovingRoles { get; set; }        
    }
}
