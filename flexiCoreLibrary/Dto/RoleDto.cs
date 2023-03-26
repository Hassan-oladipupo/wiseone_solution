using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class RoleDto
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CreatedOn { get; set; }
        public string LastUpdatedOn { get; set; }
        public bool CanAccessWeb { get; set; }
        public string Status { get; set; }
        public List<long> Functions { get; set; }
        public List<string> FunctionNames { get; set; }
    }
}
