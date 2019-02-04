using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest.Net;
using Rest.Net.Interfaces;

namespace FacebookCore
{
    public class FacebookAppApi : ApiBaseClass
    {
        public FacebookAppApi(FacebookClient client) : base(client)
        {
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {
                IRestRequest request = new RestRequest("/oauth/access_token", Http.Method.GET);
                request.AddParameter("client_id", FacebookClient.ClientId);
                request.AddParameter("client_secret", FacebookClient.ClientSecret);
                request.AddParameter("grant_type", "client_credentials");
                IRestResponse<string> result = await FacebookClient.RestClient.ExecuteAsync(request);

                var jsreader = new JsonTextReader(new StringReader(result.RawData.ToString()));
                var json = (JObject)new JsonSerializer().Deserialize(jsreader);

                string accessToken = json["access_token"].ToString(); //result.RawData.ToString().Replace("access_token=", string.Empty);
                return accessToken;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetAppId()
        {
            string accessToken = await GetAccessTokenAsync();
            return GetAppId(accessToken);
        }

        public string GetAppId(string accessToken)
        {
            try
            {
                string appId = accessToken.Split(new char[] { '|' })[0];
                return appId;
            }
            catch (Exception e)
            {
                return null;
            }
        }

/*        public FacebookAppTestUsersCollection GetTestUsers()
        {
            FacebookAppTestUsersCollection testUsers = new FacebookAppTestUsersCollection(FacebookClient);
            return testUsers;
        }

    */
    }
}
