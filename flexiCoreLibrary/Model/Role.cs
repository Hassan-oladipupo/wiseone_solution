using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Role
    {
        public Role()
        {
            this.RoleFunctions = new HashSet<RoleFunction>();
            this.Staffs = new HashSet<Staff>();
        }

        public long ID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastUpdatedOn { get; set; }

        public bool CanAccessWeb { get; set; }
        [MaxLength(20)]
        public string Status { get; set; }

        public virtual ICollection<RoleFunction> RoleFunctions { get; set; }
        public virtual ICollection<Staff> Staffs { get; set; }
    }
}
