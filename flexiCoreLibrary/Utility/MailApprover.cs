using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary
{
    public class MailApprover
    {
        public MailApprover()
        {
            this.Emails = new List<string>();
            this.Tokens = new List<string>();
        }

        public List<string> Emails { get; set; }               
        public List<string> Tokens { get; set; }
    }
}
