using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrayerTime.funcs
{
    public static class daylightSavings
    {

        public static bool isDST(DateTime beginDST, DateTime endDST, int curDateMonth, int curDateDay)
        {
            if (curDateMonth == beginDST.Month && curDateDay >= beginDST.Day)
                return true;
            else if (curDateMonth > beginDST.Month && curDateMonth < endDST.Month)
                return true;
            else if (curDateMonth == endDST.Month && curDateDay <= endDST.Day)
                return true;
            else
                return false;
        }

        public static void GetDayLightSavings(int myyear, out DateTime beginDST, out DateTime endDST)
        {
            if (myyear == 2006)
            {
                beginDST = DateTime.Parse("4/2/2006");
                endDST = DateTime.Parse("10/29/2006");
            }
            else if (myyear == 2007)
            {
                beginDST = DateTime.Parse("3/11/2007");
                endDST = DateTime.Parse("11/4/2007");
            }
            else if (myyear == 2008)
            {
                beginDST = DateTime.Parse("3/9/2008");
                endDST = DateTime.Parse("11/2/2008");
            }
            else if (myyear == 2009)
            {
                beginDST = DateTime.Parse("3/8/2009");
                endDST = DateTime.Parse("11/1/2009");
            }
            else if (myyear == 2010)
            {
                beginDST = DateTime.Parse("3/14/2010");
                endDST = DateTime.Parse("11/6/2010");
            }
            else if (myyear == 2011)
            {
                beginDST = DateTime.Parse("3/13/2011");
                endDST = DateTime.Parse("11/5/2011");
            }
            else if (myyear == 2012)
            {
                beginDST = DateTime.Parse("3/11/2012");
                endDST = DateTime.Parse("11/3/2012");
            }
            else if (myyear == 2013)
            {
                beginDST = DateTime.Parse("3/10/2013");
                endDST = DateTime.Parse("11/2/2013");
            }
            else if (myyear == 2014)
            {
                beginDST = DateTime.Parse("3/9/2014");
                endDST = DateTime.Parse("11/1/2014");
            }
            else if (myyear == 2015)
            {
                beginDST = DateTime.Parse("3/8/2015");
                endDST = DateTime.Parse("10/31/2015");
            }
            else if (myyear == 2016)
            {
                beginDST = DateTime.Parse("3/12/2016");
                endDST = DateTime.Parse("11/5/2016");
            }
            else if (myyear == 2017)
            {
                beginDST = DateTime.Parse("3/11/2017");
                endDST = DateTime.Parse("11/4/2017");
            }
            else if (myyear == 2018)
            {
                beginDST = DateTime.Parse("3/11/2018");
                endDST = DateTime.Parse("11/4/2018");
            }
            else if (myyear == 2019)
            {
                beginDST = DateTime.Parse("3/10/2019");
                endDST = DateTime.Parse("11/3/2019");
            }
            else if (myyear == 2020)
            {
                beginDST = DateTime.Parse("3/8/2020");
                endDST = DateTime.Parse("11/1/2020");
            }
            else if (myyear == 2021)
            {
                beginDST = DateTime.Parse("3/14/2021");
                endDST = DateTime.Parse("11/7/2021");
            }
            else if (myyear == 2022)
            {
                beginDST = DateTime.Parse("3/13/2022");
                endDST = DateTime.Parse("11/6/2022");
            }
            else if (myyear == 2023)
            {
                beginDST = DateTime.Parse("3/12/2023");
                endDST = DateTime.Parse("11/5/2023");
            }
            else
            {
                beginDST = new DateTime(2023, 3, 12);
                endDST = new DateTime(2023, 11, 5);
            }
/*            else if (myyear >= 2019 & mymonth >= 3)
            {
                DSTERROR = true;
                GetPrayerTimes.errorMsg = "We don't have the daylight savings for this year. Contact us to add it";
            }
            */
        }
    }
}
