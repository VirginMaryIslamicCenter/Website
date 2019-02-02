using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using VirginMaryCenter.Data;
using VirginMaryCenter.Models;
using Newtonsoft.Json;

namespace VirginMaryCenter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IStringLocalizer<HomeController> loc;
        private readonly AppConfig config;

        public HomeController(ApplicationDbContext dbContext, IStringLocalizer<HomeController> localization, AppConfig cfg)
        {
            db = dbContext;
            loc = localization;
            config = cfg;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexModel
            {
                FutureEvents = await db.GetFutureEvents(),
                Location = this.GetUserLocation()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(SubscribeModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.EmailSubscriptions.FirstOrDefaultAsync(x => x.Email == model.Email);
                if (existing == null)
                {
                    db.EmailSubscriptions.Add(new EmailSubscription
                    {
                        Email = model.Email,
                        DateAdded = DateTime.UtcNow
                    });
                    await db.SaveChangesAsync();

                    return PartialView("_SubscribeSuccess");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Email), loc["This email is already registered."]);
                }
            }

            return PartialView("_Subscribe", model);
        }

        [HttpPost]
        public IActionResult UserLocation(UserLocation model)
        {
            if (ModelState.IsValid)
            {
                // save to cookie
                Response.Cookies.Append(AppConfig.LocationCookieName, JsonConvert.SerializeObject(model, Formatting.None), new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddYears(1)
                });
                return Content("<script language='javascript' type='text/javascript'>window.location.reload(true);</script>");
            }

            return PartialView("_UserLocation", model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        private UserLocation GetUserLocation()
        {
            var cookie = Request.Cookies[AppConfig.LocationCookieName];
            return !String.IsNullOrWhiteSpace(cookie) ? JsonConvert.DeserializeObject<UserLocation>(cookie) : new UserLocation
            {
                Latitude = config.Location.Latitude,
                Longitude = config.Location.Longitude,
                Address = $"{config.Location.City}, {config.Location.State}"
            };
        }
    }
}
