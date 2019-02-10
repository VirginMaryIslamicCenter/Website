using System;
using PrayerTime.funcs;
using PrayerTime.modals;
using static PrayerTime.enumPrayerTimes;

namespace PrayerTime
{
    public static class Calculate
    {
        public static PrayerTimes GetPrayerTimes(SchoolsofThought Madhab, PrayerCalculationMethod Calc_Method, DateTime doDate, double longitude, double latitude, double timezone, bool doDayLightSavings)
        {
            PrayerTimes prayerTimes = new PrayerTimes();
            AstroValues astroValues = new AstroValues();
            int myday = doDate.Day;
            int mymonth = doDate.Month;
            int myyear = doDate.Year;

            double mj;
            double diffMin;

            DateTime curDate;

            //
            // main loop. All the work is done in the functions with the double names
            // find_sun_and_twi_events_for_date() and find_moonrise_set()
            //
            mj = JulianDay(myday, mymonth, myyear, 0);

            double imsaakAngle;
            double fajrAngle;
            double IshaAngle;

            if (Calc_Method == PrayerCalculationMethod.Karachi)
            {
                imsaakAngle = -18; fajrAngle = -18.0; IshaAngle = -18.0;
            }
            else if (Calc_Method == PrayerCalculationMethod.ISNA)
            {
                imsaakAngle = -18; fajrAngle = -15.0; IshaAngle = -15.0;
            }
            else if (Calc_Method == PrayerCalculationMethod.MuslimWorldLeague)
            {
                imsaakAngle = -18; fajrAngle = -18.0; IshaAngle = -17.0;
            }
            else if (Calc_Method == PrayerCalculationMethod.Egypt)
            {
                imsaakAngle = -19.5; fajrAngle = -19.5; IshaAngle = -17.5;
            }
            else
            {
                imsaakAngle = -18; fajrAngle = -16.0; IshaAngle = -15.0;
            }

            // Start Loop here if you want to go through dates
            astroCalcs.find_sun_and_twi_events_for_date(mj, timezone, longitude, latitude, imsaakAngle, fajrAngle, IshaAngle, astroValues);

            diffMin = Math.Abs((astroValues.sunset_end - astroValues.sunset_begin).TotalMinutes);

            curDate = new DateTime(myyear, mymonth, myday);

            getAsr getAsr = new getAsr();

            prayerTimes.Fajr = astroValues.astro_begin;
            prayerTimes.SunRise = astroValues.sunset_begin;
            prayerTimes.Dhuhr = astroValues.sunset_begin.AddMinutes(diffMin / 2); //  DateAdd("n", diffMin / 2, (DateTime)sunset_begin);;
            prayerTimes.Isha = astroValues.astro_end;

            prayerTimes.SalatUlLayl = prayerTimes.Dhuhr.AddHours(12);   // add 12 hours to zhuhr;

            if (Madhab == SchoolsofThought.Hanafi)
            {
                prayerTimes.Imsaak = astroValues.imsaak_begin;
                prayerTimes.Asr = getAsr.CalculateAsr(longitude, latitude, timezone, curDate, prayerTimes.Dhuhr, 2);
                prayerTimes.Sunset = null;
                prayerTimes.Maghrib = astroValues.sunset_end;
            }
            else if (Madhab == SchoolsofThought.Shaffii)
            {
                prayerTimes.Imsaak = astroValues.imsaak_begin;
                prayerTimes.Asr = getAsr.CalculateAsr(longitude, latitude, timezone, curDate, prayerTimes.Dhuhr, 1);
                prayerTimes.Sunset = null;
                prayerTimes.Maghrib = astroValues.sunset_end;
            }
            else
            {
                prayerTimes.Imsaak = astroValues.imsaak_begin;
                prayerTimes.Asr = getAsr.CalculateAsr(longitude, latitude, timezone, curDate, prayerTimes.Dhuhr, 1);
                prayerTimes.Sunset = astroValues.sunset_end;
                prayerTimes.Maghrib = astroValues.civil_end;

                //prayerTimes.Midnight = DateAdd(DateInterval.Minute, (DateDiff(DateInterval.Minute, (DateTime)Today + " " + sunset_end, (DateTime)Today.AddDays(1) + " " + astro_begin) / 2), sunset_end);

                prayerTimes.Midnight = astroValues.sunset_end.AddMinutes((astroValues.astro_begin.AddDays(1) - astroValues.sunset_end).TotalMinutes / 2);

            }
            getAsr = null/* TODO Change to default(_) if this is not a reference type */;

            prayerTimes.Fajr = Convert.ToDateTime(prayerTimes.Fajr); // .AddMinutes(1)
            prayerTimes.Dhuhr = Convert.ToDateTime(prayerTimes.Dhuhr).AddMinutes(1);
            prayerTimes.Isha = Convert.ToDateTime(prayerTimes.Isha); // .AddMinutes(1)

            switch (Calc_Method)
            {
                case PrayerCalculationMethod.LevaInstitute:
                case PrayerCalculationMethod.ISNA:
                    {
                        prayerTimes.Imsaak = astroValues.imsaak_begin;
                        break;
                    }

                case PrayerCalculationMethod.Karachi:
                case PrayerCalculationMethod.MuslimWorldLeague:
                case PrayerCalculationMethod.Saudi:
                case PrayerCalculationMethod.Egypt:
                    {
                        prayerTimes.Imsaak = astroValues.astro_begin.AddMinutes(-10);
                        break;
                    }
            }

            // Apply Daylight Savings time if necessary
            if (doDayLightSavings)
            {
                // DAY LIGHT SAVINGS TIME PART
                bool do_DST; // Do Day Light savings or not

                daylightSavings.GetDayLightSavings(myyear, out DateTime beginDST, out DateTime endDST);

                do_DST = daylightSavings.isDST(beginDST, endDST, mymonth, myday);

                if (do_DST == true)
                {
                    prayerTimes.Imsaak = prayerTimes.Imsaak.AddHours(1);
                    prayerTimes.Fajr = prayerTimes.Fajr.AddHours(1);
                    prayerTimes.SunRise = prayerTimes.SunRise.AddHours(1);
                    prayerTimes.Dhuhr = prayerTimes.Dhuhr.AddHours(1);
                    prayerTimes.Asr = prayerTimes.Asr.AddHours(1);
                    if (prayerTimes.Sunset.HasValue)
                        prayerTimes.Sunset = prayerTimes.Sunset.Value.AddHours(1);
                    prayerTimes.Maghrib = prayerTimes.Maghrib.AddHours(1);
                    prayerTimes.Isha = prayerTimes.Isha.AddHours(1);
                    prayerTimes.Midnight = prayerTimes.Midnight.AddHours(1);
                    prayerTimes.SalatUlLayl = prayerTimes.SalatUlLayl.AddHours(1);
                }
            }

            return prayerTimes;
        } // end of main program

        // takes decimal hours and returns a string in hhmm format
        private static string hrsmin(double hours)
        {
            double total = Math.Floor(hours * 60 + 0.5) / 60.0;

            int h = (int)Math.Floor(total);
            int m = (int)Math.Floor((60 * (total - h)) + 0.5);  // was Math.floor()

            return (h > 12 ? h - 12 : h).ToString() + ":" + (m < 10 ? "0" + m.ToString() : m.ToString()) + " " +  (h > 12 ? "pm" : "am");
        }

        //
        // round rounds the number num to dp decimal places
        // the second line is some C like jiggery pokery I
        // found in an OReilly book which means if dp is null
        // you get 2 decimal places.
        //
        public static double round(double num, double dp)
        {
            // dp = (!dp ? 2: dp)

            return Math.Round(num * (Math.Pow(10, dp))) / (Math.Pow(10, dp));    // was Math.round(
        }

        public static double JulianDay(int myday, int month, int year, int hour)
        {
            // Takes the myday, month, year and hours in the myday and returns the
            // modified julian myday number defined as mjd = jd - 2400000.5
            // checked OK for Greg era dates - 26th Dec 02
            //
            double b;

            if ((month <= 2))
            {
                month = month + 12;
                year = year - 1;
            }

            double a = 10000.0 * year + 100.0 * month + myday;
            if ((a <= 15821004.1))
                b = -2 * Math.Floor((year + 4716) / (double)4) - 1179; // was Math.floor
            else
                // b = Math.floor(year/400) - Math.floor(year/100) + Math.floor(year/4)
                b = Math.Floor(year / (double)400) - Math.Floor(year / (double)100) + Math.Floor(year / (double)4);

            a = 365.0 * year - 679004.0;

            // return
            // mjd = (a + b + Math.floor(30.6001 * (month + 1)) + myday + hour/24.0)
            return (a + b + Math.Floor(30.6001 * (month + 1)) + myday + hour / 24.0);
        }
        
    }
}