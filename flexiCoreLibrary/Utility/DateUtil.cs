using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary
{
    public class DateUtil
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public static DateTime GetDate(DateUtil dateUtil)
        {
            var dateTime = new DateTime(dateUtil.Year, dateUtil.Month, dateUtil.Day);
            return Now(dateTime);
        }

        public static DateTime Now()
        {
            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, britishZone);
            return date; 
        }

        public static DateTime Now(DateTime dateTime)
        {
            var britishZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var date = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, britishZone);
            return date;
        }
    }
}
