using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot
{
    public class PingManager
    {
        private readonly Thread worker;
        private readonly IrcManager client;
        private bool started;

        public PingManager(IrcManager _client)
        {
            worker = new Thread(new ThreadStart(Run));
            client = _client;
        }
        public void Start()
        {
            if (started)
            {
                Console.WriteLine("PingManager already started.");
                return;
            }
            worker.IsBackground = true;
            worker.Start();
            started = true;
        }
        private void Run()
        {
            while (true)
            {
                client.SendBaseMessage("PING irc.twitch.tv");
                Thread.Sleep(30000);
            }

        }

    }

}
