using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class LeaveTypeDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Deductible { get; set; }
        public bool IsFullTimeOnly { get; set; }
    }
}
