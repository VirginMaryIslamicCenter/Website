using System;
using System.Collections.Generic;
using System.Linq;

namespace VirginMaryCenter
{
    public class AppConfig
    {
        public const string LocationCookieName = "lc";

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public bool EnableFundraising { get; set; }
        public string GoogleMapsAPIKey { get; set; }

        public AppConfigLocation Location { get; set; }
        public AppConfigAbout About { get; set; }
    }

    public class AppConfigLocation
    {
        public string Venue { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TimeZone { get; set; }

        public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(TimeZone));

        public override string ToString()
        {
            return $"{Venue}, {Address}, {City}, {State}";
        }
    }

    public class AppConfigAbout
    {
        public string DefaultCulture { get; set; }
        public string AboutUs { get; set; }
        public string Twitter { get; set; }
        public string Facebook { get; set; }
    }
}
