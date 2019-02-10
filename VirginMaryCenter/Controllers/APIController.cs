using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

//using FacebookCore;
using PrayerTime;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VirginMaryCenter.Data;
using static PrayerTime.enumPrayerTimes;

namespace VirginMaryCenter.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext db;
        private readonly IStringLocalizer<HomeController> loc;
        private readonly AppConfig config;

        public APIController(ApplicationDbContext dbContext, IStringLocalizer<HomeController> localization, AppConfig cfg, IConfiguration configuration)
        {
            this.configuration = configuration;
            db = dbContext;
            loc = localization;
            config = cfg;
        }

        private const string FB_NONEXPIRING_ACCESSTOKEN = "EAALUOCj40nUBAI0mtRkTXai5zLmlPOIWrlCpIkKfNIfWgqsZCjPDdS2sFeIho86Yff05nnvrwxH74lM0buIDwBOEqZAexx7ed9zJEwgrtSNFyr3GJHRtNzFJJhduLwS7RIwBYnT9WMi99gNs4fzA4D1PrNgCwZD";

        [HttpGet]
        [Route("api/events")]
        [Produces("application/json")]
        public async Task<IActionResult> Events()
        {
            /*
             *
             * IF THIS IS NOT RETURNING RESULTS, GO TO:
             * https://www.virginmarycenter.org/admin/facebookFix
             * and get the new token and replace it here:
             *
             */

            const string FB_GRAPHURL = "https://graph.facebook.com/v3.2";
            const string FB_PAGE = "VirginMaryCenter";
            const string FB_FIELDS = "cover,description,place,attending_count,maybe_count,picture,name,end_time,event_times,is_canceled,is_draft,start_time,updated_time";

            var url = $"{FB_GRAPHURL}/{FB_PAGE}/events?fields={FB_FIELDS}&access_token={FB_NONEXPIRING_ACCESSTOKEN}";
            HttpClient hc = new HttpClient();
            string eventStr = await hc.GetStringAsync(url);

            return Ok(eventStr);
        }

        [HttpGet]
        [Route("api/prayertimes/validateLocation")]
        [Produces("application/json")]
        public async Task<IActionResult> ValidateLocation(string ZipOrCityState)
        {
            try
            {
                if (ZipOrCityState.Length == 5 && int.TryParse(ZipOrCityState, out int tmp))
                {
                    //This is a zip code
                    string sql = $"SELECT * " +
                                 $"FROM  PrayerTimes_ZipCode_cords CROSS JOIN PrayerTimes_ZipCode_citynames " +
                                 $"WHERE (PrayerTimes_ZipCode_cords.zipcode = @ZipCode) AND (PrayerTimes_ZipCode_citynames.zipcode = @ZipCode)";

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        ZipCodeInfo zipcodeDetail = await connection.QueryFirstOrDefaultAsync<ZipCodeInfo>(sql, new { ZipCode = new DbString { Value = ZipOrCityState, IsAnsi = true, IsFixedLength = true, Length = 5 } });

                        if (zipcodeDetail == null || zipcodeDetail.ZipCode == "")
                            return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMsg = "Invalid zipcode. Only US zipcodes supported right now." });

                        return Ok(zipcodeDetail);
                    }
                }
                else
                {
                    var City = "";
                    var State = "";
                    string sql = "";
                    object param = null;

                    using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                    {
                        if (ZipOrCityState.Contains(","))
                        {
                            //City, State
                            City = ZipOrCityState.Split(',')[0].Trim().ToUpper();
                            State = ZipOrCityState.Split(',')[1].Trim().ToUpper();

                            if (State.Length != 2)
                                return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMsg = "Please enter 2 letter abbreviation for the State" });

                            sql = "SELECT * " +
                                  "FROM     PrayerTimes_ZipCode_cords CROSS JOIN PrayerTimes_ZipCode_citynames " +
                                  "WHERE     (PrayerTimes_ZipCode_citynames.city = @City) AND (PrayerTimes_ZipCode_citynames.state = @State) AND (PrayerTimes_ZipCode_citynames.zipcode = PrayerTimes_ZipCode_cords.zipcode)";

                            param = new
                            {
                                City = new DbString { Value = City, IsAnsi = true },
                                State = new DbString { Value = State, IsAnsi = true }
                            };
                        }
                        else
                        {
                            //Only City
                            City = ZipOrCityState.Trim();
                            sql = "SELECT * " +
                                   "FROM     PrayerTimes_ZipCode_cords CROSS JOIN PrayerTimes_ZipCode_citynames " +
                                   "WHERE     (PrayerTimes_ZipCode_citynames.city = @City) AND (PrayerTimes_ZipCode_citynames.zipcode = PrayerTimes_ZipCode_cords.zipcode)";
                            param = new
                            {
                                City = new DbString { Value = City, IsAnsi = true }
                            };
                        }

                        IList<ZipCodeInfo> zipcodeDetail = (await connection.QueryAsync<ZipCodeInfo>(sql, param)).AsList<ZipCodeInfo>();
                        if (zipcodeDetail.Count == 0)
                            return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMsg = "Invalid city or zipcode" });

                        else
                            return Ok(zipcodeDetail);
                    }
                }
            }
            catch (Exception)
            {
            }

            return StatusCode((int)HttpStatusCode.BadRequest, new { ErrorMsg = "Invalid Zipcode or City, State" });
        }

        [HttpGet]
        [Route("api/prayertimes")]
        [Produces("application/json")]
        public IActionResult PrayerTimes(SchoolsofThought? schoolOfThought, PrayerCalculationMethod? calcMethod, DateTime? Day, double? Longitude, double? Latitude, int? GMT, bool? hasDayLightSavings)
        {
            if (!schoolOfThought.HasValue)
                schoolOfThought = SchoolsofThought.Jafari;
            if (!calcMethod.HasValue)
                calcMethod = PrayerCalculationMethod.LevaInstitute;
            if (!Day.HasValue)
                Day = DateTime.Now;
            if (!Longitude.HasValue)
                Longitude = config.Location.Longitude;
            if (!Latitude.HasValue)
                Latitude = config.Location.Latitude;
            if (!GMT.HasValue)
                GMT = config.Location.GMT;
            if (!hasDayLightSavings.HasValue)
                hasDayLightSavings = config.Location.DayLightSavings;

            return Ok(Calculate.GetPrayerTimes(schoolOfThought.Value,
                                                calcMethod.Value,
                                                Day.Value,
                                                Longitude.Value,
                                                Latitude.Value,
                                                GMT.Value,
                                                hasDayLightSavings.Value));
        }
    }
}