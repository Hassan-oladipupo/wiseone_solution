using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Model
{
    public class FeedSetting
    {
        public long ID { get; set; }
        public Enums.ServiceType FeedType { get; set; }
        public DateTime LastFeedDate { get; set; }
        public DateTime LastFeedAttempt { get; set; }
    }
}
