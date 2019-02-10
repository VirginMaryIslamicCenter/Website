using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrayerTime.modals;

namespace PrayerTime.funcs
{
    public static class astroCalcs
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <returns>SunEq_1, SunEq_2</returns>
        public static (double, double) minisun(double t)
        {
            //
            // returns the ra and dec of the Sun in an array called suneq[]
            // in decimal hours, degs referred to the equinox of date and using
            // obliquity of the ecliptic at J2000.0 (small error for +- 100 yrs)
            // takes t centuries since J2000.0. Claimed good to 1 arcmin
            //
            double p2 = 6.283185307;
            double coseps = 0.91748;
            double sineps = 0.39778;
            double M = p2 * mathfuncs.frac(0.993133 + (99.997361 * t));
            double DL = 6893.0 * Math.Sin(M) + 72.0 * Math.Sin(2 * M);
            double L = p2 * mathfuncs.frac(0.7859453 + M / p2 + (6191.2 * t + DL) / 1296000);
            double SL = Math.Sin(L);
            double X = Math.Cos(L);
            double Y = coseps * SL;
            double Z = sineps * SL;
            double RHO = Math.Sqrt(1 - Z * Z);
            double dec = (360.0 / p2) * Math.Atan(Z / RHO);
            double ra = (48.0 / p2) * Math.Atan(Y / (X + RHO));
            if ((ra < 0))
                ra = ra + 24;

            return (dec, ra); //SunEq_1, SunEq_2
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="t"></param>
        /// <returns>SunEq_1, SunEq_2</returns>
        public static (double, double) minimoon(double t)
        {
            //
            // takes t and returns the geocentric ra and dec in an array mooneq
            // claimed good to 5' (angle) in ra and 1' in dec
            // tallies with another approximate method and with ICE for a couple of dates
            //

            double p2 = 6.283185307;
            double arc = 206264.8062;
            double coseps = 0.91748;
            double sineps = 0.39778;

            double L0 = mathfuncs.frac(0.606433 + 1336.855225 * t);   // mean longitude of moon
            double L = p2 * mathfuncs.frac(0.374897 + 1325.55241 * t); // mean anomaly of Moon
            double LS = p2 * mathfuncs.frac(0.993133 + 99.997361 * t); // mean anomaly of Sun
            double D = p2 * mathfuncs.frac(0.827361 + 1236.853086 * t); // difference in longitude of moon and sun
            double F = p2 * mathfuncs.frac(0.259086 + 1342.227825 * t); // mean argument of latitude

            // corrections to mean longitude in arcsec
            double DL = 22640 * Math.Sin(L);
            DL += -4586 * Math.Sin(L - 2 * D);
            DL += +2370 * Math.Sin(2 * D);
            DL += +769 * Math.Sin(2 * L);
            DL += -668 * Math.Sin(LS);
            DL += -412 * Math.Sin(2 * F);
            DL += -212 * Math.Sin(2 * L - 2 * D);
            DL += -206 * Math.Sin(L + LS - 2 * D);
            DL += +192 * Math.Sin(L + 2 * D);
            DL += -165 * Math.Sin(LS - 2 * D);
            DL += -125 * Math.Sin(D);
            DL += -110 * Math.Sin(L + LS);
            DL += +148 * Math.Sin(L - LS);
            DL += -55 * Math.Sin(2 * F - 2 * D);

            // simplified form of the latitude terms
            double S = F + (DL + 412 * Math.Sin(2 * F) + 541 * Math.Sin(LS)) / arc;
            double H = F - 2 * D;
            double N = -526 * Math.Sin(H);
            N = N + +44 * Math.Sin(L + H);
            N = N + -31 * Math.Sin(-L + H);
            N = N + -23 * Math.Sin(LS + H);
            N = N + +11 * Math.Sin(-LS + H);
            N = N + -25 * Math.Sin(-2 * L + F);
            N = N + +21 * Math.Sin(-L + F);

            // ecliptic double and lat of Moon in rads
            double L_moon = p2 * mathfuncs.frac(L0 + DL / 1296000);
            double B_moon = (18520.0 * Math.Sin(S) + N) / arc;

            // equatorial coord conversion - note fixed obliquity
            double CB = Math.Cos(B_moon);
            double X = CB * Math.Cos(L_moon);
            double V = CB * Math.Sin(L_moon);
            double W = Math.Sin(B_moon);
            double Y = coseps * V - sineps * W;
            double Z = sineps * V + coseps * W;
            double RHO = Math.Sqrt(1.0 - Z * Z);
            double dec = (360.0 / p2) * Math.Atan(Z / RHO);
            double ra = (48.0 / p2) * Math.Atan(Y / (X + RHO));
            if ((ra < 0))
                ra = ra + 24;

            return (dec, ra); //suneq_1, suneq_2
        }

        public static double sin_alt(double iobj, double mjd0, double hour, double longitude, double clatitude, double slatitude)
        {
            //
            // this rather mickey mouse function takes a lot of
            // arguments and then returns the sine of the altitude of
            // the object labelled by iobj. iobj = 1 is moon, iobj = 2 is sun
            //
            double rads = 0.0174532925;
            double mjd = mjd0 + hour / 24.0;

            double t = (mjd - 51544.5) / 36525.0;

            double ra, dec;

            if ((iobj == 1))
                (dec, ra) = minimoon(t);
            else
                (dec, ra) = minisun(t);

            // hour angle of object
            double tau = 15.0 * (lmst(mjd, longitude) - ra);

            // produces sin(alt) of object using the conversion formulas
            return slatitude * Math.Sin(rads * dec) + clatitude * Math.Cos(rads * dec) * Math.Cos(rads * tau);
        }

        // FajrAngle and IshaAngle are based on the 5 different methods of calculating prayer (ISNA, Muslim World League, etc.)
        public static string find_sun_and_twi_events_for_date(double mjd, double timezone, double longitude, double latitude, double imsaakAngle, double FajrAngle, double IshaAngle, AstroValues astroValues)
        {
            double slatitude, mydate, ym, yz, utrise = 0, utset = 0;
            bool above, rise, sett;
            //
            // this is my attempt to encapsulate most of the program in a function
            // then this function can be generalised to find all the Sun events.
            double yp, nz, hour, z1 = 0, z2 = 0, iobj, clatitude, rads;
            rads = 0.0174532925;

            double[] sinho = new double[6] {
                Math.Sin(rads * -0.833),        // sunset upper limb simple refraction
                Math.Sin(rads * -4.0),          // civil twi
                Math.Sin(rads * -12.0),         // nautical twi                                   // Angle of Sun for Imsaak, Fajr and Isha
                Math.Sin(rads * FajrAngle),     // astro twi (-18.0)
                Math.Sin(rads * IshaAngle),     // astro twi (-18.0)
                Math.Sin(rads * imsaakAngle)    // usually -18
            };

            string always_up = " ****";
            string always_down = " ....";
            string outstring = "";
            //
            // Set up the array with the 4 values of sinho needed for the 4
            // kinds of sun event
            //
            slatitude = Math.Sin(rads * latitude);
            clatitude = Math.Cos(rads * latitude);
            mydate = mjd - timezone / 24;

            //
            // main loop takes each value of sinho in turn and finds the rise/set
            // events associated with that altitude of the Sun
            //
            for (int j = 0; j <= 5; j++)
            {
                rise = false;
                sett = false;
                above = false;
                hour = 1.0;

                ym = (double)sin_alt(2, mydate, hour - 1.0, longitude, clatitude, slatitude) - sinho[j];

                if ((ym > 0.0))
                    above = true;

                //
                // the while loop finds the sin(alt) for sets of three consecutive
                // hours, and then tests for a single zero crossing in the interval
                // or for two zero crossings in an interval or for a grazing event
                // The flags rise and sett are set accordingly
                //
                while ((hour < 25 & (sett == false || rise == false)))
                {
                    yz = sin_alt(2, mydate, hour, longitude, clatitude, slatitude) - sinho[j];
                    yp = sin_alt(2, mydate, hour + 1.0, longitude, clatitude, slatitude) - sinho[j];

                    // finds the parabola throuh the three points (-1,ym), (0,yz), (1, yp)
                    // and returns the coordinates of the max/min (if any) xe, ye
                    // the values of x where the parabola crosses zero (roots of the quadratic)
                    // and the number of roots (0, 1 or 2) within the interval [-1, 1]
                    //
                    // well, this routine is producing sensible answers
                    //
                    // results passed as array [nz, z1, z2, xe, ye]
                    //
                    double dx;

                    nz = 0;
                    double a = 0.5 * (ym + yp) - yz;
                    double b = 0.5 * (yp - ym);
                    double c = yz;
                    double xe = -b / (2 * a);
                    double ye = (a * xe + b) * xe + c;
                    double dis = b * b - 4.0 * a * c;
                    if ((dis > 0))
                    {
                        dx = 0.5 * Math.Sqrt(dis) / Math.Abs(a);
                        z1 = xe - dx;
                        z2 = xe + dx;
                        if ((Math.Abs(z1) <= 1.0))
                            nz = nz + 1;
                        if ((Math.Abs(z2) <= 1.0))
                            nz = nz + 1;
                        if ((z1 < -1.0))
                            z1 = z2;
                    }

                    // case when one event is found in the interval
                    if ((nz == 1))
                    {
                        if ((ym < 0.0))
                        {
                            utrise = hour + z1;
                            rise = true;
                        }
                        else
                        {
                            utset = hour + z1;
                            sett = true;
                        }
                    } // end of nz = 1 case

                    // case where two events are found in this interval
                    // (rare but whole reason we are not using simple iteration)
                    if ((nz == 2))
                    {
                        if ((ye < 0.0))
                        {
                            utrise = hour + z2;
                            utset = hour + z1;
                        }
                        else
                        {
                            utrise = hour + z1;
                            utset = hour + z2;
                        }
                    } // end of nz = 2 case

                    // set up the next search interval
                    ym = yp;
                    hour = hour + 2.0;
                }

                // now search has completed, we compile the string to pass back
                // to the main loop. The string depends on several combinations
                // of the above flag (always above or always below) and the rise
                // and sett flags
                //
                DateTime cur;
                if ((rise == true || sett == true))
                {
                    if ((rise == true))
                        cur = DateTime.Parse(hrsmin(utrise));
                    else
                        cur = DateTime.Parse("1/1/1900");

                    if (j == 0)
                        astroValues.sunset_begin = cur;
                    else if (j == 1)
                        astroValues.civil_begin = cur;
                    else if (j == 2)
                        astroValues.naut_begin = cur;
                    else if (j == 3)
                        astroValues.astro_begin = cur;
                    else if (j == 4)
                    {
                    }
                    else if (j == 5)
                        astroValues.imsaak_begin = cur;

                    // -------------------------------------
                    if ((sett == true))
                        cur = DateTime.Parse(hrsmin(utset));
                    else
                        cur = DateTime.Parse("1/1/1900");

                    if (j == 0)
                        astroValues.sunset_end = cur;
                    else if (j == 1)
                        astroValues.civil_end = cur;
                    else if (j == 2)
                        astroValues.naut_end = cur;
                    else if (j == 4)
                        astroValues.astro_end = cur;
                }
                else if ((above == true))
                    outstring = outstring + always_up + always_up;
                else
                    outstring = outstring + always_down + always_down;
            } // end of for loop - next condition

            return outstring;
        }

        // mjd = julian date i think
        private static double lmst(double mjd, double longitude)
        {
            // Takes the mjd and the longitude (west negative) and then returns
            // the local sidereal time in hours. Im using Meeus formula 11.4
            // instead of messing about with UTo and so on
            //
            double d = mjd - 51544.5;
            double t = d / 36525.0;
            double lst = range(280.46061837 + 360.98564736629 * d + 0.000387933 * t * t - t * t * t / 38710000);
            return (lst / 15.0 + longitude / 15);
        }


        private static double range(double x)
        {
            // returns an angle in degrees in the range 0 to 360
            double b = x / 360;
            double a = 360 * (b - mathfuncs.ipart(b));

            return a < 0 ? a + 360 : a;
        }

        // takes decimal hours and returns a string in hhmm format
        private static string hrsmin(double hours)
        {
            double total = Math.Floor(hours * 60 + 0.5) / 60.0;

            int h = (int)Math.Floor(total);
            int m = (int)Math.Floor((60 * (total - h)) + 0.5);  // was Math.floor()

            return (h > 12 ? h - 12 : h).ToString() + ":" + (m < 10 ? "0" + m.ToString() : m.ToString()) + " " + (h > 12 ? "pm" : "am");
        }

    }
}
