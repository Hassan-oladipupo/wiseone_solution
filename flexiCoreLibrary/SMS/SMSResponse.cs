using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace flexiCoreLibrary
{
    public class SMSResponse
    {
        public bool Successful { get; set; }
        public string FailCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}