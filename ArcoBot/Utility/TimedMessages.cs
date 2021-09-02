using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot
{
    public class TimedMessages
    {

        private readonly Thread worker;
        private readonly List<string> messageList;
        private readonly IrcManager client;
        private bool started;
        public TimedMessages(IrcManager _client)
        {
            client = _client;
            worker = new Thread(new ThreadStart(Run));
            messageList = new List<string>();
        }

        public void Start()
        {
            if (started)
            {
                Console.WriteLine("Timed messages have already started.");
                return;
            }
            LoadMessages();
            worker.IsBackground = true;
            worker.Start();
            started = true;
        }
        private void LoadMessages()
        {
            string[] lines = File.ReadAllLines("messages.txt");
            messageList.AddRange(lines);
        }
        public void Run()
        {
            int index = 0;
            while (true)
            {
                if (index >= messageList.Count) index = 0;
                client.SendPublicMessage(messageList[index++]);
                Thread.Sleep(180000);
            }
        }
    }

}
