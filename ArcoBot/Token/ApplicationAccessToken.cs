using ArcoBot.Interfaces;
using ArcoBot.JsonObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot.Token
{
    public class ApplicationAccessToken:IToken
    {
        private HttpClient client;
        private string expiration;
        private string accessToken;
        private string[] scopes;
        private Thread worker;
        private string clientID;
        private string clientSecret;

        const string subscriptionUri = "https://api.twitch.tv/helix/subscriptions?broadcaster_id=";
        const string baseAuthorizeUri = "https://id.twitch.tv/oauth2/authorize";
        const string tokenUri = "https://id.twitch.tv/oauth2/token";
        const string validateUri = "https://id.twitch.tv/oauth2/validate";

        public HttpClient Client { get => client; }
        public DateTime Expiration
        {
            get
            {
                if (string.IsNullOrEmpty(expiration))
                    return default;
                else
                    return Convert.ToDateTime(expiration);
            }
        }

        public string AccessToken { get => accessToken; }
        public string[] Scopes { get => scopes; }
        public Thread Worker { get => worker; }

        public ApplicationAccessToken(HttpClient client, string clientID, string clientSecret)
        {
            this.client = client;
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            worker = new Thread(new ThreadStart(CheckToken));
        }
        public void Initialize()
        {
            expiration = Configuration.Get("AppAccessExpiration");
            accessToken = Configuration.Get("AppAccessToken");
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(expiration))
                GetToken();
            else if (!CheckUri())
                GetToken();
          
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            worker.Start();
        }
        private void GetToken()
        {
            HttpRequestMessage reqMsg;
            HttpResponseMessage respMsg;
            string rawContent;
            JAppAccessToken appAccessTokenObj;

            reqMsg = new HttpRequestMessage(HttpMethod.Post, AppAccessTokenUri());
            respMsg = client.SendAsync(reqMsg).Result;
            rawContent = respMsg.Content.ReadAsStringAsync().Result;
            appAccessTokenObj = new JAppAccessToken(JsonConvert.DeserializeObject<object>(rawContent));

            accessToken = appAccessTokenObj.AccessToken;
            expiration = DateTime.Now.AddSeconds(appAccessTokenObj.Expires).ToString();
            SaveAppAccessToken();

            Console.WriteLine("Application Access Token refreshed.");
        }
        public void SaveAppAccessToken()
        {
            Configuration.Set("AppAccessToken", accessToken);
            Configuration.Set("AppAccessExpiration", expiration);
        }
        private bool CheckUri()
        {
            if (DateTime.Now > Expiration) return false;

            Uri uri = new Uri(validateUri);
            var response = client.GetAsync(uri).Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;

            dynamic jsonContent = JsonConvert.DeserializeObject<object>(responseBody);

            JResponse responseObj = new JResponse(jsonContent);
            if (responseObj.Error == null)
                return true;

            return false;

        }
        public void CheckToken()
        {
            while (true)
            {
                if (CheckUri())
                    Thread.Sleep(60000);
                else
                {
                    GetToken();
                }
            }
        }
        private string AppAccessTokenUri()
        {
            return $"{tokenUri}?client_id={clientID}&client_secret={clientSecret}" +
                $"&grant_type=client_credentials&scope=user:edit+user:edit:follows+moderation:read+user:read:blocked_users+clips:edit+channel:read:subscriptions+channel:read:hype_train+" +
                $"bits:read";
        }
    }
}
