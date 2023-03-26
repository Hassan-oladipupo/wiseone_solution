using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class FeedSettingDL
    {
        public static bool Update(FeedSetting setting)
        {
            try
            {
                using (var context = new DataContext())
                {

                    var existingSetting = context.FeedSettings
                                    .FirstOrDefault(t => t.FeedType == setting.FeedType);

                    if (existingSetting != null)
                    {
                        existingSetting.LastFeedAttempt = DateUtil.Now();
                        existingSetting.LastFeedDate = setting.LastFeedDate;

                        context.Entry(existingSetting).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    else
                    {
                        setting.LastFeedAttempt = DateUtil.Now();
                        context.FeedSettings.Add(setting);
                        context.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static FeedSetting Retrieve(Enums.ServiceType serviceType)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var feedSettings = context.FeedSettings
                                                .FirstOrDefault(t => t.FeedType == serviceType);

                    return feedSettings;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<ServicesDto> RetrieveServices()
        {
            try
            { 
                var timeSpan = new TimeSpan(0, 10, 0);
                using (var context = new DataContext())
                {
                    var services = (from service in context.FeedSettings
                                    select service)
                                    .AsEnumerable()
                                    .Select(s => new ServicesDto()
                                    {
                                        Name = Enums.GetDescription(typeof(Enums.ServiceType), s.FeedType.ToString()),
                                        LastRunDate = string.Format("{0:ddd, MMM d, yyyy h:mm tt}", s.LastFeedAttempt),
                                        Spin = DateUtil.Now().Subtract(s.LastFeedAttempt) >= timeSpan ? false : true
                                    })
                                    .ToList();

                    return services;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
