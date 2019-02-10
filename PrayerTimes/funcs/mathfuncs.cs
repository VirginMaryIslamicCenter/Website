using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerTime.funcs
{
    public static class mathfuncs
    {
        public  static int ipart(double x)
        {
            return x > 0 ? (int)Math.Floor(x) : (int)Math.Ceiling(x);
        }


        public static double frac(double x)
        {
            // returns the fractional part of x as used in minimoon and minisun
            double a = x - Math.Floor(x); // was MAth.floor
            return a < 0 ? a + 1 : a;
        }


        public static double[] quad(double ym, double yz, double yp)
        {
            //
            // finds the parabola throuh the three points (-1,ym), (0,yz), (1, yp)
            // and returns the coordinates of the max/min (if any) xe, ye
            // the values of x where the parabola crosses zero (roots of the quadratic)
            // and the number of roots (0, 1 or 2) within the interval [-1, 1]
            //
            // well, this routine is producing sensible answers
            //
            // results passed as array [nz, z1, z2, xe, ye]
            //
            double z1 = 0;
            double dx = 0;
            double z2 = 0; // , nz
            double[] quadout = new double[5];

            double nz = 0;
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

            quadout[0] = nz;
            quadout[1] = z1;
            quadout[2] = z2;
            quadout[3] = xe;
            quadout[4] = ye;

            return quadout;
        }
    }
}
