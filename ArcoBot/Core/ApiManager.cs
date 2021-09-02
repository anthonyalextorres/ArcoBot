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
    //Ideas
    //Top sub streak?
    //Timer message, join followers (count)
    public class ApiManager
    {
        private IrcManager ircManager;
        private PubSubManager pubSubManager;

        private HttpClient appAccessClient;
        private HttpClient userAccessClient;

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
        public bool Connected { get { if (ircManager != null) return ircManager.Connected; else return false; } }

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

            SetBroadcasterInfo();

            ircManager = new IrcManager(broadcasterName, userAccessToken.OAuth, broadcasterName, "irc.chat.twitch.tv", 6667);
            ircManager.Start(false);

            pubSubManager = new PubSubManager(UserAccessToken.OAuth, broadcasterID);
            pubSubManager.Initialize();

            Initialize();
        }
        public void Initialize()
        {
            InitializeSubEvents();
        }

        private void SetBroadcasterInfo()
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
            {
                broadcasterID = validationObj.UserID;
                broadcasterName = validationObj.Login;
            }

        }
        public async Task GetFollowAgeAsync(string username)
        {
            //if user does not exist?
            //If the user is the broadcaster? x

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
                if (innerJson == null)
                {
                    await SendPublicMessageAsync("That user doesn't exist!");
                    return;
                }
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

                if (followDate == default)
                {
                    await ircManager.SendPublicMessageAsync($"{displayName} isn't even a part of the Fiesta!");
                    return;
                }
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

                await SendPublicMessageAsync($"{displayName} has been a part of the Fiesta for {retnYear} {retnMonth} {retnDay}!");
                return;
            }
            await SendPublicMessageAsync("Something went wrong with getting the user's follow age!");//$"Error Found: ApiManager.GetFollowAgeAsync()";
        }
        public async Task GetSubscriberCountAsync()
        {
            if (userAccessClient == null)
            {
                await SendPublicMessageAsync("Something went wrong....");
                return;
            }

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
            await SendPublicMessageAsync($"{count} partygoers in the Fiesta Fam!!!!");
        }
        public async Task<List<string>> GetSubscriberListAsync()
        {
            if (userAccessClient == null) return null;

            string cursor;
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
                    if (sub.UserName != broadcasterName)
                        subscriberList.Add($"{sub.UserName}");
                }
                address = new Uri($"{subscriptionUri}?broadcaster_id={broadcasterID}&first=100&after={cursor}");
            }
            while (!string.IsNullOrEmpty(cursor));
            return subscriberList;
        }
        public async Task GetRandomSubAsync()
        {
            if (userAccessClient == null)
            {
                await SendPublicMessageAsync("Something went wrong....");
                return;
            }

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
            {
                await SendPublicMessageAsync($"Random appreciation to @{randomSub.UserName} for your subscription to the channel!");
                return;
            }
            await SendPublicMessageAsync("Looks like something went wrong..... scary....");

        }
        public async Task GetRandomClipAsync()
        {
            if (userAccessClient == null)
            {
                await SendPublicMessageAsync("Something went wrong....");
                return;
            }
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
            {
                await SendPublicMessageAsync($"Random clip provided! {randomClip.URL}");
                return;
            }
            await SendPublicMessageAsync("Looks like something went wrong..... scary....");
        }
        public async Task CreateClipAsync()
        {
            if (userAccessClient == null)
            {
                await SendPublicMessageAsync("Something went wrong....");
                return;
            }
            HttpRequestMessage msgReq = new HttpRequestMessage(HttpMethod.Post, $"{clipUri}?broadcaster_id={broadcasterID}");
            HttpResponseMessage msgResp = await userAccessClient.SendAsync(msgReq);
            string content = await msgResp.Content.ReadAsStringAsync();

            dynamic jsonContent = JsonConvert.DeserializeObject<object>(content);
            if (jsonContent != null)
            {
                JClipCreation jClip = new JClipCreation(jsonContent["data"].First());
                {
                    await SendPublicMessageAsync("Clip created!");
                    await SendPublicMessageAsync($"{ jClip.EditURL.Replace("/edit", "")}");
                    return;
                }
            }
            await SendPublicMessageAsync("Uh oh.... We couldn't create the clip!");
        }
        public async Task GetClipThumbnail()
        {
            if (userAccessClient == null)
            {
                await SendPublicMessageAsync("Something went wrong with getting the clip thumbnail!");
                return;
            }


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
            {
                await SendPublicMessageAsync($"Go find that clip! {randomClip.ThumbnailURL}");
                return;
            }
            await SendPublicMessageAsync("Looks like something went wrong..... scary....");
            return;
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

        public async Task GetModeratorsAsync()
        {
            if (userAccessClient == null) { await SendPublicMessageAsync("Something went wrong...."); return; }

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

            await SendPublicMessageAsync(retn);
        }
        public async Task CreatePollAsync(string title, string[] choices, bool pointsEnabled, int pointsCost, int duration)
        {
            //Needs work. Cleanup code? gui feature?
            if (userAccessClient == null) { await SendPublicMessageAsync("Something went wrong... arcotvSad arcotvSad arcotvSad"); return; }
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitch.tv/helix/polls");

            JPollCreate poll = new JPollCreate();
            poll.BroadcasterID = broadcasterID;
            poll.Title = title;
            poll.Choices = new JPollChoice[choices.Length];
            for (int i = 0; i < choices.Length; i++)
                poll.Choices[i] = new JPollChoice() { Title = choices[i] };
            poll.ChannelPointsVotingEnabled = pointsEnabled;
            poll.ChannelPointsPerVote = pointsCost;
            poll.Duration = duration * 60;
            request.Content = new StringContent(JsonConvert.SerializeObject(poll));
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var response = await userAccessClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            await SendPublicMessageAsync($"Poll started! {poll.Title}");

        }
        public async Task<string> CreatePredictionAsync()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the current uptime of the stream then sends a message to chat.
        /// </summary>
        private async Task GetUptimeAsync()
        {

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

        public async Task Ban(string username)
        {
            await ircManager.SendPublicMessageAsync($".ban { username}");
            await ircManager.SendPublicMessageAsync($"{username} has been banned for a party foul!");
        }

        public void InitializeSubEvents()
        {
            pubSubManager.OnPointsRedeemed += OnPointsRedeemed;
            pubSubManager.OnFollow += OnFollow;
            pubSubManager.OnSubscribe += OnSubscribe;
            pubSubManager.OnCheer += OnCheer;
            pubSubManager.OnBitsBadgeUnlock += OnBitsBadgeUnlock;

        }

        private void OnSubscribe(object sender, PubSub.Events.OnSubscribeArgs e)
        {
            ircManager.SendPublicMessage($"{e.DisplayName} has subscribed with a {e.SubPlanName} subscription!!!");
        }

        private void OnFollow(object sender, PubSub.Events.OnFollowArgs e)
        {
            ircManager.SendPublicMessage($"{e.DisplayName} has joined the Fiesta Fam!!!");
        }

        #region PubSub Events
        private void OnPointsRedeemed(object sender, PubSub.Events.OnChannelPointsRedeemedArgs e)
        {
            ircManager.SendPublicMessage($"{e.Redemption.User.DisplayName} has redeemed {e.Redemption.Reward.Title} for {e.Redemption.Reward.Cost} points!!!");
        }

        private void OnCheer(object sender, PubSub.Events.OnCheerArgs e)
        {
            ircManager.SendPublicMessage($"TO DOOOOOO");
        }
        private void OnBitsBadgeUnlock(object sender, PubSub.Events.OnBitsBadgeUnlocksArgs e)
        {
            ircManager.SendPublicMessage($"TO DOOOOOO");
        }
        #endregion

        #region IRC

        public void SendPublicMessage(string message)
        {
            ircManager.SendPublicMessage(message);
        }
        private async Task SendPublicMessageAsync(string message)
        {
            await ircManager.SendPublicMessageAsync(message);
        }

        public async Task<string> ReadMessage()
        {
            string msg = ircManager.ReadMessage();
            await HandleCommand(msg);

            return msg;
        }
        private async Task HandleCommand(string msg)
        {
            if (ircManager != null)
            {
                if (!string.IsNullOrEmpty(msg))

                    if (msg.Contains("PRIVMSG"))
                    {
                        string submsg = msg.Substring(1);
                        int nameIndex = submsg.IndexOf('!');
                        string userName = submsg.Substring(0, nameIndex);
                        int indexParse = submsg.IndexOf(':') + 1;
                        submsg = submsg.Substring(indexParse);
                        string[] cmdArgs = submsg.Split(' ');
                        if (cmdArgs[0].StartsWith("!"))
                        {
                            switch (cmdArgs[0])
                            {
                                case "!poll":
                                    {
                                        //Cleanup with sanity checks
                                        if (cmdArgs.Length < 6)
                                        {
                                            await ircManager.SendPublicMessageAsync("Command formatted incorrectly. Example: !poll Will_Arco_Win? 2 Yes No 3");
                                            break;
                                        }
                                        string title = cmdArgs[1].Replace('_', ' ');
                                        string[] choices = new string[Convert.ToInt32(cmdArgs[2])];
                                        int index = 3;
                                        for (int i = 0; i < choices.Length; i++)
                                        {
                                            choices[i] = cmdArgs[3 + i];
                                            index++;
                                        }
                                        await CreatePollAsync(title, choices, false, 0, Convert.ToInt32(cmdArgs[index]));

                                    }
                                    break;
                                case "!mods":
                                    await GetModeratorsAsync();
                                    break;

                                case "!followage":
                                    if (cmdArgs.Length == 2)
                                        await GetFollowAgeAsync(cmdArgs[1]);
                                    else
                                        await GetFollowAgeAsync(userName);
                                    break;

                                case "!subs":
                                    await GetSubscriberCountAsync();
                                    break;

                                case "!randomsub":
                                    await GetRandomSubAsync();
                                    break;

                                case "!randomclip":
                                    await GetRandomClipAsync();
                                    break;

                                case "!clip":
                                    await CreateClipAsync();
                                    break;

                                case "!randomcliphunt":
                                    await GetClipThumbnail();
                                    break;

                                default: 
                                    SendPublicMessage("I don't think that command exists! arcotvSad arcotvSad arcotvSad "); 
                                    break;
                            }
                        }
                    }
            }
        }
    }
    #endregion
}