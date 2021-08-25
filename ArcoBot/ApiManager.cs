using ArcoBot.JsonObjects;
using ArcoBot.Token;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ArcoBot
{
    public class ApiManager
    {
        public IrcClient ircClient;
        private HttpClient appAccessClient;
        private HttpClient userAccessClient;

        private PubSubManager pubSubManager;
  
        private UserAccessToken userAccessToken;
        private ApplicationAccessToken appAccessToken;

        private string broadcasterID = default;//"190862879"
        private string broadcasterName = default;

        const string baseUri = "https://api.twitch.tv/helix/";
        const string tokenUri = "https://id.twitch.tv/oauth2/token";
        const string authorizeUri = "https://id.twitch.tv/oauth2/authorize";
        const string followUri = "https://api.twitch.tv/helix/users/follows";
        const string validateUri = "https://id.twitch.tv/oauth2/validate";
        const string clipUri = "https://api.twitch.tv/helix/clips";
        const string userUri = "https://api.twitch.tv/helix/users";
        const string subscriptionUri = "https://api.twitch.tv/helix/subscriptions";
        private string viewerUri = "https://tmi.twitch.tv/group/user/arcotv/chatters";

        private static string clientID;
        private static string clientSecret;

        public UserAccessToken UserAccessToken { get => userAccessToken; }
        public ApplicationAccessToken AppAccessToken { get => appAccessToken; }

        public ApiManager()
        {
            //TODO: Load emote strings from file
            clientID = Configuration.Get("ClientID");
            clientSecret = Configuration.Get("ClientSecret");

            appAccessClient = new HttpClient();
            appAccessClient.DefaultRequestHeaders.Add("Client-ID", clientID);
            appAccessClient.DefaultRequestHeaders.Add("Accept", "application/json");

            userAccessClient = new HttpClient();
            userAccessClient.DefaultRequestHeaders.Add("Client-ID", clientID);
            userAccessClient.DefaultRequestHeaders.Add("Accept", "application/json");

            userAccessToken = new UserAccessToken(userAccessClient, clientID, clientSecret);
            userAccessToken.Initialize();

            appAccessToken = new ApplicationAccessToken(appAccessClient, clientID, clientSecret);
            appAccessToken.Initialize();

            SetBroadcasterID();

            pubSubManager = new PubSubManager(UserAccessToken.OAuth, broadcasterID);
            pubSubManager.Initialize();

            Initialize();
        }
        public void Initialize()
        {
            InitializeSubEvents();
        }

        public string GetBroadcasterID()
        {
            return broadcasterID;
        }
        public void SetIRCClient(IrcClient client)
        {
            if (client != null) ircClient = client;
            //throw some exception
        }
        private string UserAccessTokenUri()
        {
            return $"{ authorizeUri }?client_id={ clientID}" +
             "&redirect_uri=http://localhost:8080&response_type=code" +
                 $"&scope=user:edit+user:edit:follows+user:read:blocked_users+moderation:read+clips:edit+channel:read:subscriptions+channel:read:hype_train+" +
                 $"bits:read+chat:read+channel:read:redemptions+chat:edit&force_verify=true";
        }
        private string AppAccessTokenUri()
        {
            return $"{tokenUri}?client_id={clientID}&client_secret={clientSecret}" +
                $"&grant_type=client_credentials&scope=user:edit+user:edit:follows+user:read:blocked_users+clips:edit+channel:read:subscriptions+channel:read:hype_train+" +
                $"bits:read";
        }
        private string UserAccessTokenRequest(string code)
        {
            return $"{tokenUri}?client_id={clientID}&client_secret={clientSecret}" +
                $"&code={code}&grant_type=authorization_code&redirect_uri=http://localhost:8080";
        }
        public void SetBroadcasterID()
        {
            JValidation validationObj;
            HttpResponseMessage respMsg;
            string rawContent;
            dynamic jsonContent;

            respMsg = userAccessClient.GetAsync(validateUri).Result;
            rawContent = respMsg.Content.ReadAsStringAsync().Result;
            jsonContent = JsonConvert.DeserializeObject<object>(rawContent);
            validationObj = new JValidation(jsonContent.ToString());
            if (validationObj != null)
                broadcasterID = validationObj.UserID;

        }
        public async Task<string> GetFollowAgeAsync(string username)
        {
            //if user does not exist?
            //If the user is the broadcaster?
           
            if (appAccessClient != null)
            {
                DateTime followDate = default;
                DateTime currentFollowDate;
                DateTime now;
                HttpResponseMessage respMsg1;
                HttpResponseMessage respMsg2;
                string rawContent1;
                string rawContent2;
                dynamic jsonContent1;
                dynamic jsonContent2;
                string callID;
                string displayName;
                string cursor;
                int years;
                int months = default;
                int days;
                string fromGet;
                string address;


                fromGet = "https://api.twitch.tv/helix/users/follows?limit=100&from_id=";
                address = fromGet;

                respMsg1 = await appAccessClient.GetAsync($"{userUri}?login={username}");
                rawContent1 = await respMsg1.Content.ReadAsStringAsync();

                jsonContent1 = JsonConvert.DeserializeObject<object>(rawContent1);
                dynamic innerJson = jsonContent1["data"].First;
                if (innerJson == null) return "That user doesn't exist!";
                JUser callUser = new JUser(innerJson);
                callID = callUser.ID;
                displayName = callUser.DisplayName;

                address = $"{fromGet}{callID}";
                do
                {
                    respMsg2 = await appAccessClient.GetAsync($"{address}");
                    rawContent2 = await respMsg2.Content.ReadAsStringAsync();
                    jsonContent2 = JsonConvert.DeserializeObject<object>(rawContent2);

                    JPagination pagination = new JPagination(jsonContent2["pagination"]);
                    cursor = pagination.Cursor;

                    foreach (var follower in jsonContent2["data"])
                    {
                        JFollow followedUser = new JFollow(follower);
                        if (followedUser != null)
                        {
                            if (followedUser.ToName.ToLower() != "arcotv")
                                continue;

                            else
                            {
                                followDate = followedUser.FollowDate;
                                break;
                            }
                        }
                    }
                    address = $"{fromGet}{callID}&after={cursor}";
                }
                while (!string.IsNullOrEmpty(cursor));

                if (followDate == default) return $"{displayName} isn't even a part of the Fiesta!";

                now = DateTime.UtcNow;
                years = new DateTime(DateTime.UtcNow.Subtract(followDate).Ticks).Year - 1;
                currentFollowDate = followDate.AddYears(years);

                for (int i = 1; i <= 12; i++)
                {
                    if (currentFollowDate.AddMonths(i) == now)
                    {
                        months = i;
                        break;
                    }
                    else if (currentFollowDate.AddMonths(i) >= now)
                    {
                        months = i - 1;
                        break;
                    }
                }
                days = now.Subtract(currentFollowDate.AddMonths(months)).Days;

                //Format return string
                string retnYearBase = years > 1 ? "years" : "year";
                string retnYear = years > 0 ? $"{years} {retnYearBase}" : string.Empty;

                string retnMonthBase = months > 1 ? "months" : "month";
                string retnMonth = months > 0 ? $"{months} {retnMonthBase}" : string.Empty;

                string retnDayBase = days > 1 ? "days" : "day";
                string retnDay = years > 0 ? $"{days} {retnDayBase}" : string.Empty;

                return $"{displayName} has been a part of the Fiesta for {retnYear} {retnMonth} {retnDay}!";
            }
            return $"Error Found: ApiManager.GetFollowAgeAsync()";
        }
        public async Task<string> GetSubscriberCountAsync()
        {
            if (userAccessClient == null) return string.Empty;

            string cursor = string.Empty;
            int count = -1;
            Uri address = new Uri($"{ subscriptionUri }?broadcaster_id={broadcasterID}&first=100");

            do
            {
                var response = await userAccessClient.GetAsync(address);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
                JPagination pagination = new JPagination(responseObject["pagination"]);
                cursor = pagination.Cursor;
                foreach (var subscriber in responseObject["data"])
                {
                    JSubscription sub = new JSubscription(subscriber);
                    count++;
                }
                address = new Uri($"{subscriptionUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            return count.ToString();
        }
        public async Task<List<string>> GetSubscriberListAsync()
        {
            if (userAccessClient == null) return null;

            string cursor = string.Empty;
            int count = -1;
            Uri address = new Uri($"{ subscriptionUri }?broadcaster_id={broadcasterID}&first=100");
            List<string> subscriberList = new List<string>();
            do
            {
                var response = await userAccessClient.GetAsync(address);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
                JPagination pagination = new JPagination(responseObject["pagination"]);
                cursor = pagination.Cursor;
                foreach (var subscriber in responseObject["data"])
                {
                    JSubscription sub = new JSubscription(subscriber);
                    if (sub.UserName != Global.Channel)
                        subscriberList.Add($"{sub.UserName}");
                }
                address = new Uri($"{subscriptionUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            return subscriberList;
        }
        public async Task<string> GetRandomSubAsync()
        {
            if (userAccessClient == null) return string.Empty;

            string cursor = string.Empty;
            List<JSubscription> subList = new List<JSubscription>();
            Uri address = new Uri($"{ subscriptionUri }?broadcaster_id={broadcasterID}&first=100");
            do
            {
                var response = await userAccessClient.GetAsync(address);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
                JPagination pagination = new JPagination(responseObject["pagination"]);
                cursor = pagination.Cursor;
                foreach (var subscriber in responseObject["data"])
                {
                    JSubscription sub = new JSubscription(subscriber);
                    subList.Add(sub);
                }
                address = new Uri($"{subscriptionUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            JSubscription randomSub;
            Random ranGen = new Random();
            randomSub = subList[ranGen.Next(0, subList.Count - 1)];
            if (randomSub != null)
                return $"Random appreciation to @{randomSub.UserName} for your subscription to the channel!";
            return "Looks like something went wrong..... scary....";

        }
        public async Task<string> GetRandomClipAsync()
        {
            if (userAccessClient == null) return string.Empty;

            string cursor = string.Empty;
            List<JClip> clipList = new List<JClip>();
            Uri address = new Uri($"{ clipUri }?broadcaster_id={broadcasterID}&first=100");
            do
            {
                var response = await userAccessClient.GetAsync(address);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
                JPagination pagination = new JPagination(responseObject["pagination"]);
                cursor = pagination.Cursor;
                foreach (var clip in responseObject["data"])
                {
                    JClip clipObj = new JClip(clip);
                    clipList.Add(clipObj);
                }
                address = new Uri($"{clipUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            JClip randomClip;
            Random ranGen = new Random();
            randomClip = clipList[ranGen.Next(0, clipList.Count - 1)];
            Console.WriteLine(randomClip.ToString());
            if (randomClip != null)
                return $"Random clip provided! {randomClip.URL}";
            return "Looks like something went wrong..... scary....";
        }
        public async Task<string> CreateClipAsync()
        {
            if (userAccessClient == null) return "Something went wrong....";

            HttpRequestMessage msgReq = new HttpRequestMessage(HttpMethod.Post, $"{clipUri}?broadcaster_id={broadcasterID}");
            HttpResponseMessage msgResp = await userAccessClient.SendAsync(msgReq);
            string content = await msgResp.Content.ReadAsStringAsync();
            
            dynamic jsonContent = JsonConvert.DeserializeObject<object>(content);
            if (jsonContent != null)
            {
                JClipCreation jClip = new JClipCreation(jsonContent["data"].First());
                return $"{ jClip.EditURL.Replace("/edit", "")}";
            }
            return "Uh oh.... We couldn't create the clip!";
        }
        public async Task<string> GetClipThumbnail()
        {
            if (userAccessClient == null) return string.Empty;

            string cursor = string.Empty;
            List<JClip> clipList = new List<JClip>();
            Uri address = new Uri($"{ clipUri }?broadcaster_id={broadcasterID}&first=100");
            do
            {
                var response = await userAccessClient.GetAsync(address);
                var responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
                JPagination pagination = new JPagination(responseObject["pagination"]);
                cursor = pagination.Cursor;
                foreach (var clip in responseObject["data"])
                {
                    JClip clipObj = new JClip(clip);
                    clipList.Add(clipObj);
                }
                address = new Uri($"{clipUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            JClip randomClip;
            Random ranGen = new Random();
            randomClip = clipList[ranGen.Next(0, clipList.Count - 1)];
            if (randomClip != null)
                return $"Go find that clip! {randomClip.ThumbnailURL}";
            return "Looks like something went wrong..... scary....";
        }
        public async Task<List<string>> GetCurrentViewers()
        {
            if (userAccessClient == null)
            {
                Console.WriteLine("UserAccessClient is null. Unable to make user calls.");
                return null;
            }

            List<string> viewerList = new List<string>();
            Dictionary<string, string> viewerDict = new Dictionary<string, string>();
            var msgReq = await userAccessClient.GetAsync(viewerUri);
            var msgResp = await msgReq.Content.ReadAsStringAsync();
            var viewers = new JViewerList(msgResp);

            //Add key as name, value as type?
            foreach (var mem in viewers.Chatters.Admins)
            {
                viewerList.Add($"{mem}");
                viewerDict.Add(mem, "Admins");
            }

            foreach (var mem in viewers.Chatters.Broadcaster)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "Broadcaster");
            }

            foreach (var mem in viewers.Chatters.GlobalMods)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "Global Moderators");
            }

            foreach (var mem in viewers.Chatters.Moderators)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "Moderators");
            }

            foreach (var mem in viewers.Chatters.Staff)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "Staff");
            }

            foreach (var mem in viewers.Chatters.Viewers)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "Viewers");
            }

            foreach (var mem in viewers.Chatters.VIPs)
            {
                viewerList.Add(mem);
                viewerDict.Add(mem, "VIPs");
            }

            return viewerList;
        }

        public async Task<string> GetModeratorsAsync()
        {
            if (userAccessClient == null) return "Something went wrong...";

            string retn = "Much love to all the wonderful Moderators of the Fiesta Fam: ";
            HttpResponseMessage response = await userAccessClient.GetAsync($"https://api.twitch.tv/helix/moderation/moderators?broadcaster_id={ broadcasterID }");//new HttpRequestMessage(HttpMethod.Get, "https://api.twitch.tv/helix/moderation/moderators");
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);
            int count = responseObject["data"].Count;

            foreach (var moderator in responseObject["data"])
            {
                JModerator mod = new JModerator(moderator);
                if (mod.UserName != "Streamlabs")
                {
                    if (responseObject["data"][count - 1] == moderator)
                        retn += mod.UserName + "! arcotvDrink arcotvDrink arcotvDrink ";
                    else
                        retn += mod.UserName + ", ";
                }
            }

            return retn;
        }
        public async Task<string> CreatePollAsync(string title, string[] choices, bool pointsEnabled, int pointsCost, int duration)
        {
            //Needs work. Cleanup code? gui feature?
            if (userAccessClient == null) return "Something went wrong... arcotvSad arcotvSad arcotvSad";
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitch.tv/helix/polls");
        
            JPollCreate poll = new JPollCreate();
            poll.BroadcasterID = broadcasterID;
            poll.Title = title;
            poll.Choices = new JPollChoice[choices.Length];
            for (int i = 0; i < choices.Length; i++)
                poll.Choices[i] = new JPollChoice() { Title = choices[i]};
            poll.ChannelPointsVotingEnabled = pointsEnabled;
            poll.ChannelPointsPerVote = pointsCost;
            poll.Duration = duration * 60;
            request.Content = new StringContent(JsonConvert.SerializeObject(poll));
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await userAccessClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            return $"Poll started! {poll.Title}";

        }
        public async Task<string> CreatePredictionAsync()
        {
            return string.Empty;
        }
        public async Task StartCommercialAsync()
        {
            //POST https://api.twitch.tv/helix/channels/commercial

            if (userAccessClient == null) return;
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitch.tv/helix/channels/commercial");
            JCommercial commercial = new JCommercial(broadcasterID, "60");
            request.Content = new StringContent(JsonConvert.SerializeObject(commercial));
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await userAccessClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Yes");


        }

        public async Task<string> SetTitleAsync(string title) { return string.Empty; }
        public async Task<string> SetGameAsync(string gameTitle)
        {
            //spaces in name?
            if (userAccessClient == null) return "Something went wrong... arcotvSad arcotvSad arcotvSad";
            HttpResponseMessage response = await userAccessClient.GetAsync($"https://api.twitch.tv/helix/games?name={gameTitle}");
            string responseBody = await response.Content.ReadAsStringAsync();
            dynamic responseObject = JsonConvert.DeserializeObject<object>(responseBody);

            return string.Empty;
        }

        public void InitializeSubEvents()
        {
            pubSubManager.OnPointsRedeemed += OnPointsRedeemed;
            pubSubManager.OnFollow += OnFollow;
            pubSubManager.OnSubscribe += OnSubscribe;

        }

        private void OnSubscribe(object sender, PubSub.Events.OnSubscribeArgs e)
        {
            ircClient.SendPublicMessage($"{e.DisplayName} has subscribed with a {e.SubPlanName} subscription!!!");
        }

        private void OnFollow(object sender, PubSub.Events.OnFollowArgs e)
        {
            ircClient.SendPublicMessage($"{e.DisplayName} has joined the Fiesta Fam!!!");
        }

        #region PubSub Events
        private void OnPointsRedeemed(object sender, PubSub.Events.OnChannelPointsRedeemedArgs e)
        {
            ircClient.SendPublicMessage($"{e.Redemption.User.DisplayName} has redeemed {e.Redemption.Reward.Title} for {e.Redemption.Reward.Cost} points!!!");
        }
        #endregion
    }
}