using ArcoBot.JsonObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot
{
    public class LurkBot
    {
        Thread systemSleeper;
        private HttpClient httpClient;
        public IrcManager ircClient;
        private string apiAddress = "https://api.twitch.tv/helix/streams/?game_id=33214&first=100";
        private List<JLiveStream> streamList;
        private ConcurrentQueue<JLiveStream> streamQueue;
        public LurkBot(IrcManager _client)
        {
            ircClient = _client;
            httpClient = new HttpClient();
            streamList = new List<JLiveStream>();
            streamQueue = new ConcurrentQueue<JLiveStream>();
            systemSleeper = new Thread(new ThreadStart(Run));
            systemSleeper.IsBackground = true;
        }
        public async void Start()
        {
            string address = apiAddress;
            string cursor = "";

            httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
          //  httpClient.DefaultRequestHeaders.Add("Client-ID", Global.clientid);
          //  httpClient.DefaultRequestHeaders.Authorization =
            //    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Global.AppAccessToken);


            ircClient.Start(true);
            systemSleeper.Start();

            do
            {
                var responseMessage = await httpClient.GetAsync(address);
                var responseContent = await responseMessage.Content.ReadAsStringAsync();

                dynamic obj = JsonConvert.DeserializeObject<object>(responseContent);
                cursor = obj["pagination"]["cursor"];

                foreach (var item in obj["data"])
                {
                    if (item == null) continue;

                    JLiveStream stream = new JLiveStream(item);
                    if (stream != null)
                    {
                        stream.user_name = stream.user_name.ToLower();
                        streamList.Add(stream);
                        streamQueue.Enqueue(stream);
                    }
                }
                address = $"{ apiAddress }&after={cursor}";
            }
            while (!string.IsNullOrEmpty(cursor));
        }
        private void Run()
        {
            try
            {
                while (true)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        if (streamQueue != null)
                        {
                            JLiveStream stream;
                            if (streamQueue.TryDequeue(out stream))
                            {
                                ircClient.Join(stream.user_name);
                            }
                        }
                    }
                    Thread.Sleep(15000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
