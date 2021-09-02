using ArcoBot.JsonObjects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot
{
    public class IrcManager
    {
        private Regex regex = new Regex("^(?:[:@]([^\\s]+) )?([^\\s]+)(?: ((?:[^:\\s][^\\s]* ?)*))?(?: ?:(.*))?$");//Might need later? https://stackoverflow.com/questions/8490084/trying-to-write-an-irc-client-but-struggling-to-find-a-good-resource-regarding-c
        private string username;
        private string password;
        private string channel;
        private string address;
        private int port;
        private bool started;
        /// <summary>
        /// The object that reads the data that the server sends.
        /// </summary>
        private readonly StreamReader inputStream;
        /// <summary>
        /// The object the sends data back to the server.
        /// </summary>
        private readonly StreamWriter outputStream;
        private readonly TcpClient client;
        private readonly PingManager pinger;
        private readonly TimedMessages messages;
        private readonly Thread worker;
        private readonly ConcurrentQueue<string> messageList;
        public bool Connected { get { if (client != null) return client.Connected; else return false; } }

        /// <summary>
        /// Constructor for Main Channel Bot
        /// </summary>
        /// <param name="_username"></param>
        /// <param name="_password"></param>
        /// <param name="_channel"></param>
        /// <param name="_address"></param>
        /// <param name="_port"></param>
        public IrcManager(string _username, string _password, string _channel, string _address, int _port)
        {
            username = _username;
            password = _password;
            channel = _channel;
            address = _address;
            port = _port;

            client = new TcpClient(address, port);
            inputStream = new StreamReader(client.GetStream());
            outputStream = new StreamWriter(client.GetStream()) { AutoFlush = true, NewLine = "\r\n" };
            messageList = new ConcurrentQueue<string>();
            worker = new Thread(new ThreadStart(RunMessages));
            messages = new TimedMessages(this);
            pinger = new PingManager(this);

        }
        public IrcManager(string _username, string _password, string _address, int _port)
        {
            username = _username;
            password = _password;
            address = _address;
            port = _port;

            client = new TcpClient(address, port);
            inputStream = new StreamReader(client.GetStream());
            outputStream = new StreamWriter(client.GetStream()) { AutoFlush = true, NewLine = "\r\n" };
            messageList = new ConcurrentQueue<string>();
            worker = new Thread(new ThreadStart(RunMessages));
            pinger = new PingManager(this);

        }
        public void Start(bool lurk)
        {
            if (started)
            {
                Console.WriteLine("Error: IrcClient already instantiated.");
                return;
            }

            outputStream.WriteLine($"PASS {password}");
            outputStream.WriteLine($"NICK {username}");
            outputStream.WriteLine($"USER {username} 8 * :{username}");
            if (!lurk)
            {
                outputStream.WriteLine($"JOIN #{channel}");
                messages.Start();
            }
            pinger.Start();
            worker.Start();
            started = true;
        }
        public void Join(string channelName)
        {
            //problem
            outputStream.WriteLine($"JOIN #{channelName}");
        }

        public void Leave()
        {
            outputStream.WriteLine($"PART #{channel}");
        }

        public void SendBaseMessage(string message)
        {
            try
            {
                outputStream.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task SendBaseMessageAsync(string message)
        {
            try
            {
                await outputStream.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public void SendPublicMessage(string message)
        {
            try
            {
                string sendmsg = $":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channel} :{message}";
                SendBaseMessage(sendmsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task SendPublicMessageAsync(string message)
        {
            try
            {
                string sendmsg = $":{username}!{username}@{username}.tmi.twitch.tv PRIVMSG #{channel} :{message}";
                await SendBaseMessageAsync(sendmsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public string ReadMessage()
        {
            try
            {
                if (messageList != null)
                {
                    string msg;
                    if (messageList.TryDequeue(out msg))
                        return msg;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return string.Empty;
        }
        private void RunMessages()//run like timed messages?
        {
            while (true)
            {
                try
                {
                    if (messageList != null)
                        messageList.Enqueue(inputStream.ReadLine());
                }
                catch (System.IO.IOException) { }
                Thread.Sleep(100);
            }
        }
    }
}