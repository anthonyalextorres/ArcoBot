using ArcoBot.Enums;
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
    public class UserAccessToken : IToken
    {
        private HttpClient client;
        private string expiration;
        private string accessToken;
        private string[] scopes;
        private Thread worker;
        private string clientID;
        private string clientSecret;
        private string refreshToken;
        private string broadcasterID;

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
        public string OAuth { get => $"oauth:{accessToken}"; }
        public UserAccessToken(HttpClient client, string clientID, string clientSecret)
        {
            this.client = client;
            this.clientID = clientID;
            this.clientSecret = clientSecret;
            worker = new Thread(new ThreadStart(CheckToken));
        }
        public void Initialize()
        {
            //Check token validity?
            expiration = Configuration.Get("UserAccessExpiration");
            accessToken = Configuration.Get("UserAccessToken");
            refreshToken = Configuration.Get("UserAccessRefreshToken");
            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(expiration))
                GetToken(false);
            else if (!CheckUri())
                GetToken(true);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            SetBroadcasterID();
            worker.Start();
        }
        private void GetToken(bool refresh)
        {
            HttpRequestMessage reqMsg;
            HttpResponseMessage respMsg;
            string rawContent;
            JUserAccessToken userAccessTokenObj;

            if (refresh)
            {
                reqMsg = new HttpRequestMessage(HttpMethod.Post, UserAccessTokenRefreshRequest());
                respMsg = client.SendAsync(reqMsg).Result;
                rawContent = respMsg.Content.ReadAsStringAsync().Result;
                userAccessTokenObj = new JUserAccessToken(JsonConvert.DeserializeObject<object>(rawContent));

                accessToken = userAccessTokenObj.AccessToken;
                refreshToken = userAccessTokenObj.RefreshToken;
                expiration = DateTime.Now.AddSeconds(userAccessTokenObj.Expires).ToString();
                SaveUserAccessToken();
                Console.WriteLine("User Access Token refreshed.");
            }
            else
            {
                Login authenticator = new Login(FullAuthorizeUri());
                authenticator.ShowDialog();

                if (authenticator.Code != string.Empty)
                {
                    reqMsg = new HttpRequestMessage(HttpMethod.Post, UserAccessTokenRequest(authenticator.Code));
                    respMsg = client.SendAsync(reqMsg).Result;
                    rawContent = respMsg.Content.ReadAsStringAsync().Result;
                    userAccessTokenObj = new JUserAccessToken(JsonConvert.DeserializeObject<object>(rawContent));

                    accessToken = userAccessTokenObj.AccessToken;
                    refreshToken = userAccessTokenObj.RefreshToken;
                    expiration = DateTime.Now.AddSeconds(userAccessTokenObj.Expires).ToString();

                    SaveUserAccessToken();
                    Console.WriteLine("User Access Token refreshed.");
                }
                else
                {

                    //throw some shit?
                    //JResponse object
                }
            }
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
                   GetToken(true);
                }
            }
        }
        public void SaveUserAccessToken()
        {
            Configuration.Set("UserAccessToken", accessToken);
            Configuration.Set("UserAccessRefreshToken", refreshToken);
            Configuration.Set("UserAccessExpiration", expiration);
        }
        private void SetBroadcasterID()
        {
            JValidation validationObj;
            HttpResponseMessage respMsg;
            string rawContent;
            dynamic jsonContent;

            respMsg = client.GetAsync(validateUri).Result;
            rawContent = respMsg.Content.ReadAsStringAsync().Result;
            jsonContent = JsonConvert.DeserializeObject<object>(rawContent);
            validationObj = new JValidation(jsonContent.ToString());
            if (validationObj != null)
                broadcasterID = validationObj.UserID;
        }
        private string FullAuthorizeUri()
        {
            return $"{ baseAuthorizeUri }?client_id={ clientID }" +
                   "&redirect_uri=http://localhost:8080&response_type=code" +
                    $"&scope=user:edit+user:edit:follows+user:read:blocked_users+clips:edit+channel:read:subscriptions+channel:read:hype_train+" +
                    $"bits:read+chat:read+moderation:read+chat:edit+channel:manage:polls+channel:manage:predictions+channel:read:polls+" +
                    $"channel:read:predictions+channel:read:redemptions+channel:edit:commercial&force_verify=true";
        }
        private string UserAccessTokenRequest(string code)
        {
            return $"{tokenUri}?client_id={clientID}&client_secret={clientSecret}" +
                $"&code={code}&grant_type=authorization_code&redirect_uri=http://localhost:8080";
        }
        private string UserAccessTokenRefreshRequest()
        {
            return $"{tokenUri}?client_id={clientID}&client_secret={clientSecret}" +
                $"&grant_type=refresh_token&refresh_token={refreshToken}";
            /*
                POST https://id.twitch.tv/oauth2/token
    ? grant_type = refresh_token
    & refresh_token = eyJfaWQmNzMtNGCJ9 % 6VFV5LNrZFUj8oU231 / 3Aj
           & client_id = fooid
           & client_secret = barbazsecret
                */
        }

    }
}
