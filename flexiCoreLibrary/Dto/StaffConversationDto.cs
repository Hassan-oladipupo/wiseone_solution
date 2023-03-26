using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Dto
{
    public class StaffConversationDto
    {
        public StaffConversationDto()
        {
            this.Conversations = new List<ConversationDto>();
        }

        public long StaffID { get; set; }
        public string StaffPicture { get; set; }
        public string StaffName { get; set; }
        public DateTime Date { get; set; }
        public string DateSent { get; set; }
        public string MessageText { get; set; }
        public List<ConversationDto> Conversations { get; set; }        
    }
}
