using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VirginMaryCenter.Controllers
{
    [Route("api/[controller]")]
    public class APIController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        [Route("api/events")]
        public async Task<string> Events()
        {
            const string FB_GRAPHURL = "https://graph.facebook.com/v3.2";
            const string FB_ACCESSTOKEN = "EAALUOCj40nUBABSmAYJMIyHHkvvhTzfplEeJaXm9JmRIXBK0WLpK31ZCGno5Tyylb0fz1sj9p7YZCvc2GlFjrc2z6xZCZCTuLOUJNVVA6X60FEHmigrg4XZAzh0fnNNREWKMCKNIsHOuB7FdFsoZCX8EP66eyI4mQG60r5wNabMN7aJwlqGLSqInZCZCHZANc93IGLiZCl7rfUzgZDZD";
            const string FB_PAGE = "VirginMaryCenter";
            const string FB_FIELDS = "cover%2Cdescription%2Cplace%2Cattending_count%2Cmaybe_count%2Cpicture";
            HttpClient hc = new HttpClient();
            string eventStr = await hc.GetStringAsync($"{FB_GRAPHURL}/{FB_PAGE}/events?fields={FB_FIELDS}&access_token={FB_ACCESSTOKEN}"); 

            return eventStr;
        }
    }
}
