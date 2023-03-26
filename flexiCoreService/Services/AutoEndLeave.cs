using flexiCoreLibrary;
using flexiCoreLibrary.Model;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace flexiCoreService.Services
{
    public class AutoEndLeave
    {
        public static void Run()
        {
            try
            {
                var time = (DateUtil.Now()).Hour;

                if (time >= 21)
                {
                    FinancialYearPL.AutoLeave(Enums.AutoLeaveType.End);
                }

                var feed = new FeedSetting()
                {
                    FeedType = Enums.ServiceType.AutoEndLeave,
                    LastFeedAttempt = DateUtil.Now(),
                    LastFeedDate = (DateUtil.Now()).Date
                };
                FeedSettingPL.SaveFeedSettings(feed);
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }
    }
}