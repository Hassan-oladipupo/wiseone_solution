using flexiCoreLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace flexiCoreService.Services
{
    public class KeepAlive
    {
        public static void Register()
        {
            new KeepAlive().Echo("KeepAlive", 1);
        }

        private static CacheItemRemovedCallback OnCacheRemove = null;

        private void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            try
            {
                SendEcho();                
                Echo(k, Convert.ToInt32(v));
            }
            catch (Exception ex)
            {
                ErrorHandler.WriteError(ex);
            }
        }

        private void Echo(string name, int minutes)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, minutes, null,
                DateTime.Now.AddMinutes(minutes), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        private void SendEcho()
        {
            var keepAliveService = new KeepAliveServiceRef.KeepAliveService();
            keepAliveService.Echo();
        }
    }
}