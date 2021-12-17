using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Salvo.Utilities.ZoneTime
{
    public class ZoneTime
    {
        public static DateTime Change(DateTime? date, string strZone)
        {
            //var list = TimeZoneInfo.GetSystemTimeZones().ToList(); Argentina Standard Time
            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(strZone);
            DateTime returnDate = TimeZoneInfo.ConvertTimeFromUtc((DateTime)date, zone);
            return returnDate;
        }
    }
}
