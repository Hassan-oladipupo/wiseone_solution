using flexiCoreLibrary;
using flexiCoreLibrary.Model;
using flexiCoreLibrary.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Services;

namespace flexiCoreService
{
    /// <summary>
    /// Summary description for KeepAliveService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class KeepAliveService : System.Web.Services.WebService
    {

        [WebMethod]
        public void Echo()
        {
            var feedSetting = new FeedSetting()
            {
                FeedType = Enums.ServiceType.KeepAlive,
                LastFeedAttempt = DateUtil.Now(),
                LastFeedDate = (DateUtil.Now()).Date
            };

            FeedSettingPL.SaveFeedSettings(feedSetting);
        }
    }
}
