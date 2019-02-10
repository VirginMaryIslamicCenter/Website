using PrayerTime.funcs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerTime
{
    public class getAsr
    {
        /* Copyright 2007 Islamic Awareness
         * Written with the help of Oleg Kharatinov
         * Edited in 2019 for new features in C# and Math library
         * Based on:
         * http://squ1.org/wiki/Solar_Position_Calculator
         * http://www.waukesha.k12.wi.us/South/EarthScience/AngleOfTheSun/AngleOfTheSun.shtml
         */

     
        // Constants
        private double PI = 3.14159265358979;
        private double degreesToRadians = 3.14159265358979 / 180;


        // You give it the Shadow Length
        // Returns: Sun's Angle from Horizon
        private double CalculateSunAngle(double shadow)
        {
            return Math.Atan(1 / shadow) * 180 / PI;
        }

        // You give the Altitude of the Sun
        // Returns: Shadow length of object
        private double GetShadowLength(double Altitude)
        {
            return 1 / Math.Tan(Altitude * degreesToRadians);
        }

        
        public DateTime CalculateAsr(double Longitude, double Latitude, double TimeZone, DateTime curDate, DateTime ZhuhrTime, Int16 ShadowLength)
        {
            // Location in radians.
            double fLatitude, fLongitude, fTimeZone;
            // Calculated data.
            double fDifference, fDeclination, fEquation, fLocalTime;
            // Solar information.
            double fLocalTime2, fSolarTime, fSolarTime1, fSolarTime2, fAltitude, fAzimuth, fSunrise, fSunset;
            // Integer values.
            double iJulianDate, t, m, hour, minute;
            // Temp data.
            double test;


            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // ' READ INPUT DATA
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // Get location data.
            // NB: this data is hard-coded for now.
            fLatitude = degreesToRadians * (Latitude);
            fTimeZone = degreesToRadians * 15 * (TimeZone); // TimeZone -8 for our tests (Pacific)
            fLongitude = degreesToRadians * (Longitude);


            // Get julian date.
            // This is also hard-coded.  I have no idea how to deal with dates.
            iJulianDate = Math.Abs((new DateTime(curDate.Year, 1, 1) - curDate).TotalDays);

            // Check the date
            iJulianDate = iJulianDate % 365;

            while (iJulianDate < 0)
                iJulianDate = iJulianDate + 365;

            // Get local time value.
            // This variable represents the number of hours since midnight.
            // So 14:30 would be 14.5.
            fLocalTime = datefuncs.ParseTime(ZhuhrTime);

            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // ' CALCULATE SOLAR VALUES
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            t = 2 * PI * ((iJulianDate - 1) / 365);

            fDeclination = (0.322003
                  - 22.971 * Math.Cos(t)
                  - 0.357898 * Math.Cos(2 * t)
                  - 0.14398 * Math.Cos(3 * t)
                  + 3.94638 * Math.Sin(t)
                  + 0.019334 * Math.Sin(2 * t)
                  + 0.05928 * Math.Sin(3 * t));

            // Cap the values
            if (fDeclination < -89.9 || fDeclination > 89.9)
                fDeclination = 89.9;

            // Convert to Radians
            fDeclination = fDeclination * degreesToRadians;

            // Calculate the equation of time as per Carruthers et al.
            t = (279.134 + 0.985647 * iJulianDate) * degreesToRadians;

            fEquation = (5.0323
                  - 100.976 * Math.Sin(t)
                  + 595.275 * Math.Sin(2 * t)
                  + 3.6858 * Math.Sin(3 * t)
                  - 12.47 * Math.Sin(4 * t)
                  - 430.847 * Math.Cos(t)
                  + 12.5024 * Math.Cos(2 * t)
                  + 18.25 * Math.Cos(3 * t));

            // Convert seconds to hours.
            fEquation = fEquation / 3600;

            // Calculate difference (in minutes) from reference longitude.
            fDifference = (fLongitude - fTimeZone) / PI * 12;

            // Convert solar noon to local noon.
            double local_noon = 12 - fEquation - fDifference;

            // Calculate angle normal to meridian plane.
            if ((fLatitude > (0.99 * (PI / 2))))
                fLatitude = (0.99 * (PI / 2));
            if ((fLatitude < -(0.99 * (PI / 2))))
                fLatitude = -(0.99 * (PI / 2));

            test = -Math.Tan(fLatitude) * Math.Tan(fDeclination);

            if ((test < -1))
                t = Math.Acos(-1) / (15 * degreesToRadians);
            else if ((test > 1))
                t = Math.Acos(1) / (15 * degreesToRadians);
            else
                t = Math.Acos(-Math.Tan(fLatitude) * Math.Tan(fDeclination)) / (15 * degreesToRadians);

            // Sunrise and sunset.
            fSunrise = local_noon - t;
            fSunset = local_noon + t;

            // Check validity of local time.
            if ((fLocalTime > fSunset))
                fLocalTime = fSunset;
            if ((fLocalTime < fSunrise))
                fLocalTime = fSunrise;
            if ((fLocalTime > 24))
                fLocalTime = 24;
            if ((fLocalTime < 0))
                fLocalTime = 0;

            // Caculate solar time.
            fSolarTime = fLocalTime + fEquation + fDifference;

            // Calculate hour angle.
            double fHourAngle = (15 * (fSolarTime - 12)) * degreesToRadians;

            // Calculate current altitude.
            t = (Math.Sin(fDeclination) * Math.Sin(fLatitude)) + (Math.Cos(fDeclination) * Math.Cos(fLatitude) * Math.Cos(fHourAngle));
            fAltitude = Math.Asin(t);

            // Calculate current azimuth.
            // t = (Sin(fDeclination) * Cos(fLatitude)) _
            // - (Cos(fDeclination) * Sin(fLatitude) _
            // * Cos(fHourAngle))
            // 
            // ' Avoid division by zero error.
            // If (fAltitude < (PI / 2)) Then
            // sin1 = (-Cos(fDeclination) * Sin(fHourAngle)) / Cos(fAltitude)
            // cos2 = t / Cos(fAltitude)
            // Else
            // sin1 = 0
            // cos2 = 0
            // End If
            // 
            // ' Some range checking.
            // If (sin1 > 1) Then sin1 = 1
            // If (sin1 < -1) Then sin1 = -1
            // If (cos2 < -1) Then cos2 = -1
            // If (cos2 > 1) Then cos2 = 1


            // Calculate azimuth subject to quadrant.
            // If (sin1 < -0.99999) Then
            // fAzimuth = ArcSin(sin1)
            // ElseIf ((sin1 > 0) And (cos2 < 0)) Then
            // If (sin1 >= 1) Then fAzimuth = -(PI / 2) Else fAzimuth = (PI / 2) + ((PI / 2) - ArcSin(sin1))
            // ElseIf ((sin1 < 0) And (cos2 < 0)) Then
            // If (sin1 <= -1) Then fAzimuth = (PI / 2) Else fAzimuth = -(PI / 2) - ((PI / 2) + ArcSin(sin1))
            // Else
            // fAzimuth = ArcSin(sin1)
            // End If

            // A little last-ditch range check.
            // If ((fAzimuth < 0) And (fLocalTime < 10)) Then fAzimuth = -fAzimuth




            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            // ' FORMAT OUTPUT DATA
            // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            // Response.Write "Time: " & HoursToString(fLocalTime) & "<br>"

            // Response.Write "Declination: " & (fDeclination * 180 / PI) & "<br>"
            // Response.Write "Altitude at Zhuhr: " & (fAltitude * 180 / PI) & "<br>"

            double ShadowLengthZhuhr = GetShadowLength(fAltitude * 180 / PI);

            // Response.Write "Shadow Length at Zhuhr: " & ShadowLengthZhuhr & "<BR>"

            double ShadowLengthAsr = ShadowLengthZhuhr + ShadowLength;

            double SunAngleAsr = CalculateSunAngle(ShadowLengthAsr);


            // -------------------------------------------
            // Do it the other way
            // Given: Altitude; want: current time



            // shadowNoon = shadowlength(42.52)
            // AsrShadow = shadowNoon + 2 '(+2 for hanafi, +1 for shafii)
            // AsrAltitude = CalculateSunAngle(AsrShadow)



            // Response.Write "Shadow Length at 12:23:  " & shadowlength(42.52) & "<BR><BR>"
            // Response.write "Sun Angle at Asr: " & AsrAltitude & "<BR><BR>"


            double Altitude = SunAngleAsr; // From Above calculation

            fAltitude = degreesToRadians * (Altitude);

            t = (Math.Sin(fAltitude) - Math.Sin(fDeclination) * Math.Sin(fLatitude)) / (Math.Cos(fDeclination) * Math.Cos(fLatitude));
            fHourAngle = Math.Acos(t);

            // fSolarTime1 = -fHourAngle / degreesToRadians / 15 + 12
            fSolarTime2 = fHourAngle / degreesToRadians / 15 + 12;

            // fLocalTime1 = fSolarTime1 - fDifference - fEquation
            fLocalTime2 = fSolarTime2 - fDifference - fEquation;
            

            return datefuncs.HoursToDateTime(fLocalTime2);
        }

    }
}
