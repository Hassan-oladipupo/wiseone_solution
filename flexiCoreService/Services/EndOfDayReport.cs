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
    public class EndOfDayReport
    {        
        public static void Run()
        {
            try
            {
                var time = (DateUtil.Now()).Hour;

                if (time >= 20 && time < 21)
                {
                    SignInOutPL.PrepareEndOfDayReport();
                    SignInOutPL.PrepareEndOfDayReport(Enums.ReportType.MissingClockOutReport);
                }

                var feed = new FeedSetting()
                {
                    FeedType = Enums.ServiceType.EndOfDayReport,
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