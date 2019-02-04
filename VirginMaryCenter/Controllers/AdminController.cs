using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirginMaryCenter.Code;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VirginMaryCenter.Controllers
{
    public class AdminController : Controller
    {


        [HttpGet, HttpPost]
        [Route("admin/facebookFix")]
        public async Task<IActionResult> FacebookFix(string userAccess = "")
        {
            //I got this from here:
            //https://gist.github.com/Bonno/96a5d7d71198c533eb5f46e5a00ae7a9
            /*
             1. Make sure you are the admin of the FB page you wish to pull info from
             2. Create a FB App (should be with the same user account that is the page admin)
             3. Head over to the Facebook Graph API Explorer
             4. On the top right, select the FB App you created from the "Application" drop down list
             5. Click "Get Access Token"
             6. Make sure you add the manage_pages permission
             7. Convert this short-lived access token into a long-lived one by making this Graph API call: https://graph.facebook.com/oauth/access_token?client_id=<your FB App ID >&client_secret=<your FB App secret>&grant_type=fb_exchange_token&fb_exchange_token=<your short-lived access token>
             8. Grab the new long-lived access token returned back
             9. Make a Graph API call to see your accounts using the new long-lived access token: https://graph.facebook.com/me/accounts?access_token=<your long-lived access token>
            10. Grab the access_token for the page you'll be pulling info from
            11. Lint (https://developers.facebook.com/tools/debug/accesstoken/) the token to see that it is set to Expires: Never!
            */



            if (userAccess != "")
            {
                var url = "https://" + $"graph.facebook.com/oauth/access_token?client_id={FacebookCredentials.FB_APPID}&client_secret={FacebookCredentials.FB_SECRET}&grant_type=fb_exchange_token&fb_exchange_token=";
                HttpClient hc = new HttpClient();
                string eventStr = await hc.GetStringAsync($"{url}{userAccess}");

                string accesstoken = "";
                dynamic obj = JsonConvert.DeserializeObject<object>(eventStr);
                accesstoken = obj["access_token"];

                if (eventStr.Contains("expiring") || eventStr.Contains("expire"))
                {
                    eventStr = await hc.GetStringAsync($"{url}{accesstoken}");
                    obj = JsonConvert.DeserializeObject<object>(eventStr);
                    accesstoken = obj["access_token"];
                }

                return Ok("Put this Non-Expiring token in your code: " + accesstoken);

                //https://graph.facebook.com/oauth/access_token?client_id=796287624073845&client_secret=0db9795dd06aede0290b0266493c79e0&grant_type=fb_exchange_token&fb_exchange_token=EAALUOCj40nUBAC9GtF6B82sOQXvcd0u7uZBsj8ZCy9d2CYZAo6sU6KmlvILGbAYqwRXWGfBtunz01ViIQ9K41bNqex63oyZAJIwZCCSFZCMtFW92d1DZCc1ZCxzhTpwbYM2lUInfG7MwyTCp9rvSLkAuCdsAvhwZCUOAZD
            }

            return View();
        }
        
    }
}
