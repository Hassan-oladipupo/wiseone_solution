using flexiCoreLibrary;
using flexiCoreLibrary.Model;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace flexiCoreService.Services
{
    public class SendDailyReportMail
    {
        public static void Run()
        {
            try
            {
                var time = (DateUtil.Now()).Hour;

                if (time >= 21)
                {
                    Mail.SendAbsentReportMail();
                    Mail.SendAbsentReportMail(Enums.ReportType.MissingClockOutReport);
                }

                var feed = new FeedSetting()
                {
                    FeedType = Enums.ServiceType.SendDailyReportMail,
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