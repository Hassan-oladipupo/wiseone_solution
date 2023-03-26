using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.PushNotification
{
    public class Message
    {
        public Notification notification { get; set; }
        public MessageContent data { get; set; }
        public List<string> registration_ids { get; set; }
        public string priority { get; set; }
    }   
}
