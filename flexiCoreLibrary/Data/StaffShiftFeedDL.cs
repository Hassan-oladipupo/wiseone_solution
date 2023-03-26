using flexiCoreLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Data
{
    public class StaffShiftFeedDL
    {
        static object _lock = new object();

        public static bool Save(List<StaffShiftFeed> latestFeeds)
        {
            try
            {
                lock (_lock)
                {
                    using (var context = new DataContext())
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                var existingFeeds = context.StaffShiftFeeds;
                                context.StaffShiftFeeds.RemoveRange(existingFeeds);
                                context.SaveChanges();

                                context.StaffShiftFeeds.AddRange(latestFeeds);
                                context.SaveChanges();

                                transaction.Commit();
                                return true;
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<StaffShiftFeed> RetrieveLatestFeeds(long locationID)
        {
            try
            {
                using (var context = new DataContext())
                {
                    var feeds = context.StaffShiftFeeds
                                        .Where(c => c.LocationID == locationID)
                                        .ToList();
                    return feeds;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
