using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerTime.modals
{
    public class PrayerTimes
    {
        public string errorMsg;       // if their is an error in the computation

        public DateTime Imsaak;
        public DateTime Fajr;
        public DateTime SunRise;

        public DateTime Dhuhr;
        public DateTime Asr;

        public DateTime? Sunset;
        public DateTime Maghrib;
        public DateTime Isha;

        public DateTime Midnight;

        public DateTime SalatUlLayl;

        public bool isSunsetEqualMaghrib;
    }

}
