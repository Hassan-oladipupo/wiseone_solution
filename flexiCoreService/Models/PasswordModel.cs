using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace flexiCoreService.Models
{
    public class PasswordModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public long StaffId { get; set; }
    }
}