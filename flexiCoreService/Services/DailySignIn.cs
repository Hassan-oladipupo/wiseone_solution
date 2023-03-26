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
    public class DailySignIn
    {        
        public static void Run()
        {          
            try
            {
                var prepareLatestFeeds = false;

                var lastFeed = FeedSettingPL.RetrieveFeedSettings(Enums.ServiceType.DailySignIn);
                if (lastFeed == null)
                {
                    prepareLatestFeeds = true;
                }
                else
                {
                    var lastFeedDate = lastFeed.LastFeedDate;
                    var today = (DateUtil.Now()).Date;
                    if (today > lastFeedDate)
                    {
                        prepareLatestFeeds = true;
                    }
                }

                if (prepareLatestFeeds)
                {
                    StaffShiftFeedPL.PrepareShiftFeedsForTheDay();
                    if (lastFeed == null)
                    {
                        lastFeed = new FeedSetting();
                    }

                    lastFeed.LastFeedDate = (DateUtil.Now()).Date;
                    lastFeed.LastFeedAttempt = DateUtil.Now();
                    lastFeed.FeedType = Enums.ServiceType.DailySignIn;
                }

                FeedSettingPL.SaveFeedSettings(lastFeed);                
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }              
    }
}