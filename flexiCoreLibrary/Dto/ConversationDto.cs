using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class ConversationDto
    {
        public string MessageText { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool SentByMe { get; set; }
    }
}
