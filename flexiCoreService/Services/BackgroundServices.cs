using flexiCoreLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;

namespace flexiCoreService.Services
{
    public class BackgroundServices
    {
        public static void Register()
        {
            var backgroundServices = new BackgroundServices();
            backgroundServices.RegisterService("RunWiseOneServices", 4);
        }

        private static CacheItemRemovedCallback OnCacheRemove = null;        

        private void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            try
            {
                RunServices();
                // re-add our task so it recurs
                RegisterService(k, Convert.ToInt32(v));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }

        private void RegisterService(string name, int minutes)
        {            
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, minutes, null,
                DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        private void RunServices()
        {           
            flexiCoreLibrary.Utility.Util.RunServicesInParallel(
                    () => DailySignIn.Run(),
                    () => AutoStartLeave.Run(),
                    () => AutoEndLeave.Run(),
                    () => EndOfDayReport.Run(),
                    () => SendDailyReportMail.Run()
                );            
        }

        
    }
}