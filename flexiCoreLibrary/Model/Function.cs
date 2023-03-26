using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class Function
    {
        public Function()
        {
            this.RoleFunctions = new HashSet<RoleFunction>();
        }

        public long ID { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }

        [MaxLength(50)]
        public string PageUrl { get; set; }

        [MaxLength(50)]
        public string Module { get; set; }

        public virtual ICollection<RoleFunction> RoleFunctions { get; set; }
    }
}
