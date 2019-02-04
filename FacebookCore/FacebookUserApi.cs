using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace FacebookCore
{
    public class FacebookUserApi : ApiBaseClass
    {
        private readonly string _authToken;

        public FacebookUserApi(FacebookClient client, string authToken) : base(client)
        {
            _authToken = authToken;
        }
        
        public async Task<JObject> RequestInformation(string[] fields = null)
        {
            string fieldsStr = string.Empty;
            if (fields != null)
            {
                fieldsStr = string.Join(",", fields);
            }
            var response = await FacebookClient.GetAsync($"/me?fields={fieldsStr}", _authToken);
            return response;
        }

        public async Task<JObject> RequestMetaData()
        {
            var response = await FacebookClient.GetAsync($"/{FacebookClient.GraphApiVersion}/me?metadata=1", _authToken);
            return response;
        }
    }
}
