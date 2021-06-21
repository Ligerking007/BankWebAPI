using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Common
{
    public static class StringTools
    {
        public static DateTime StringToDate(this string date)
        {
            //20180831            
            int year = Convert.ToInt16(date.Substring(0, 4));
            int month = Convert.ToInt16(date.Substring(4, 2));
            int day = Convert.ToInt16(date.Substring(6, 2));

            return new DateTime(year, month, day);
        }

        public static string DateToString(this DateTime date)
        {
            return string.Format("{0:yyyyMMdd}", date);
        }

        public static string TimeToString(this DateTime date)
        {
            return string.Format("{0:HHmmss}", date);
        }

        public static string RandomString(int length = 16)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
