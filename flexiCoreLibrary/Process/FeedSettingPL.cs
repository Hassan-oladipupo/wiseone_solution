using flexiCoreLibrary.Data;
using flexiCoreLibrary.Dto;
using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Process
{
    public class FeedSettingPL
    {
        public static bool SaveFeedSettings(FeedSetting setting)
        {
            try
            {
                return FeedSettingDL.Update(setting);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static FeedSetting RetrieveFeedSettings(Enums.ServiceType serviceType)
        {
            try
            {
                return FeedSettingDL.Retrieve(serviceType);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static List<ServicesDto> RetrieveServices()
        {
            try
            {
                return FeedSettingDL.RetrieveServices();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
