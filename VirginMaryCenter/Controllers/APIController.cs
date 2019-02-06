using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using FacebookCore;
using Newtonsoft.Json.Linq;
using VirginMaryCenter.Code;

namespace VirginMaryCenter.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
       
        /*IF THIS IS NOT RETURNING RESULTS, GO TO:
        https://www.virginmarycenter.org/admin/facebookFix
        and get the new token and replace it here:
             */  
        const string FB_NONEXPIRING_ACCESSTOKEN = "EAALUOCj40nUBAI0mtRkTXai5zLmlPOIWrlCpIkKfNIfWgqsZCjPDdS2sFeIho86Yff05nnvrwxH74lM0buIDwBOEqZAexx7ed9zJEwgrtSNFyr3GJHRtNzFJJhduLwS7RIwBYnT9WMi99gNs4fzA4D1PrNgCwZD";
        
        // GET: api/<controller>
        [HttpGet]
        [Route("api/events")]
        [Produces("application/json")]
        public async Task<IActionResult> Events()
        {
            //var fb = new FacebookClient(FacebookCredentials.FB_APPID, FacebookCredentials.FB_SECRET, FacebookCredentials.FB_VER);
            
            const string FB_GRAPHURL = "https://graph.facebook.com/v3.2";
            const string FB_PAGE = "VirginMaryCenter";
            const string FB_FIELDS = "cover,description,place,attending_count,maybe_count,picture,name,end_time,event_times,is_canceled,is_draft,start_time,updated_time";

            var url = $"{FB_GRAPHURL}/{FB_PAGE}/events?fields={FB_FIELDS}&access_token={FB_NONEXPIRING_ACCESSTOKEN}";
            HttpClient hc = new HttpClient();
            string eventStr = await hc.GetStringAsync(url); 

            return Ok(eventStr);
        }



    }
}
