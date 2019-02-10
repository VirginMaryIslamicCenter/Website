using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerTime
{
    public static class enumPrayerTimes
    {

        public enum SchoolsofThought
        {
            Jafari = 0,
            Hanafi = 1,
            nonHanafi = 2,
            Maliki = 2,
            Shaffii = 2,
            Hanbali = 2
        }

        public enum PrayerCalculationMethod
        {
            LevaInstitute = 0 // Leva Research Instititute in Qom 
    ,
            Karachi = 1,
            ISNA = 2,
            MuslimWorldLeague = 3,
            Saudi = 4,
            Egypt = 5
        }

    }
}
