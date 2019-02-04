using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FacebookCore;
using Newtonsoft.Json.Linq;

namespace VirginMaryCenter.Controllers
{
    [ApiController]
    public class APIController : ControllerBase
    {
        // GET: api/<controller>
        [HttpGet]
        [Route("api/events")]
        [Produces("application/json")]
        public async Task<JObject> Events()
        {

            //https://graph.facebook.com/oauth/access_token?client_id=796287624073845&client_secret=0db9795dd06aede0290b0266493c79e0&grant_type=client_credentials

            //https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=796287624073845&client_secret=0db9795dd06aede0290b0266493c79e0&fb_exchange_token=

            var fb = new FacebookClient("796287624073845", "0db9795dd06aede0290b0266493c79e0", "v3.2");
            //var fb = new FacebookClient("796287624073845", "Q7WVUiavGeq1QUMINDnZGz-B6xQ", "v3.2");
            string FB_ACCESSTOKEN = await fb.App.GetAccessTokenAsync();

            var a = fb.CreateUserApi(FB_ACCESSTOKEN);
            

            var r = await fb.GetAsync("/VirginMaryCenter?fields=access_token", FB_ACCESSTOKEN);

            return r;


            var str = fb.CreateUserApi(FB_ACCESSTOKEN).RequestInformation();

            // var t = fb.CreateUserApi(FB_ACCESSTOKEN);

            //FB_ACCESSTOKEN = str;


            const string FB_GRAPHURL = "https://graph.facebook.com/v3.2";
            //const string FB_ACCESSTOKEN = "EAALUOCj40nUBABSmAYJMIyHHkvvhTzfplEeJaXm9JmRIXBK0WLpK31ZCGno5Tyylb0fz1sj9p7YZCvc2GlFjrc2z6xZCZCTuLOUJNVVA6X60FEHmigrg4XZAzh0fnNNREWKMCKNIsHOuB7FdFsoZCX8EP66eyI4mQG60r5wNabMN7aJwlqGLSqInZCZCHZANc93IGLiZCl7rfUzgZDZD";
            //                             EAALUOCj40nUBAHtGIKetAZC19ZAktJlmk3ta51xFb6jVkkXXiW8ns2LkBoZCLZC7EJcQP6jwuVvG3QZBPZBJcX1VehgaFk7FPumCTieFtUowHEUXoZBWcfHRyG3xZCVREK3benBztbUp3ztsozMtBnMNVCgAwHZC9kthHBEP1z1WEm328eKvj7XsC4I0xhM34WX1L9T49NrHgUbUslnt9lXCINzf7ZC4kCuZATZB6E1nG5uKsQZDZD

//https://graph.facebook.com/v3.2/VirginMaryCenter/events?fields=cover%2Cdescription%2Cplace%2Cattending_count%2Cmaybe_count%2Cpicture&access_token=EAALUOCj40nUBAFlVGzMk3cWw4SBjJ49zu0MEZA1zd5dkIBVSgGf23ttmSDizlA7yzjZAjDLQ4rrHIK05UDGNGjRYQOSGaB0iLZA9JwbjFHeoU1QXcizt7715W2uWRZBfiC8S0HZBroHoo9ax03z2vTnMZCxzS9hq85JFw5ALvM2yRxUaIqowCZAxKeLBWOFkbIZD"
//https://graph.facebook.com/v3.2/VirginMaryCenter/events?fields=cover%2Cdescription%2Cplace%2Cattending_count%2Cmaybe_count%2Cpicture&access_token=796287624073845|Q7WVUiavGeq1QUMINDnZGz-B6xQ
            const string FB_PAGE = "VirginMaryCenter";
            const string FB_FIELDS = "cover%2Cdescription%2Cplace%2Cattending_count%2Cmaybe_count%2Cpicture";

            var url = $"{FB_GRAPHURL}/{FB_PAGE}/events?fields={FB_FIELDS}&access_token={FB_ACCESSTOKEN}";
            HttpClient hc = new HttpClient();
            string eventStr = await hc.GetStringAsync(url); 

            //return eventStr;
        }
    }
}
