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
    public class AutoStartLeave
    {
        public static void Run()
        {
            try
            {
                FinancialYearPL.AutoLeave();

                var feed = new FeedSetting()
                {
                    FeedType = Enums.ServiceType.AutoStartLeave,
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