using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace flexiCoreLibrary.Extensions
{
    public static class FormatExtension
    {
        public static string FormatDate(this string date)
        {
            var formattedDate = "";

            if(date.Contains("-"))
            {
                var dateComponents = date.Split('-');

                formattedDate = $"{dateComponents[2]}/{dateComponents[1]}/{dateComponents[0]}";
            }
            else
            {
                formattedDate = date;
            }

            return formattedDate;
        }
    }
}
