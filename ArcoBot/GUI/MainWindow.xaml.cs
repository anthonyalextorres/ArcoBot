using ArcoBot.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArcoBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IrcClient chatClient;
        public IrcClient lurkClient;
        public LurkBot lurkBot;
        public System.Timers.Timer chatTabTimer;
        public System.Timers.Timer lurkerTabTimer;
        public System.Timers.Timer viewerListTimer;
        public static ApiManager apiManager;
        public static PubSubManager pubSubManager;
        TextBoxOutputter outputter;

        public MainWindow()
        {
            InitializeComponent();

            apiManager = new ApiManager();
            chatClient = new IrcClient(Global.Channel, apiManager.UserAccessToken.OAuth, Global.Channel, Global.address, Global.port);
            apiManager.SetIRCClient(chatClient);

           

            // Enter the listening loop.
            //outputter = new TextBoxOutputter(chatTxtBox.);
            //Console.SetOut(outputter);
        }
        
        private async void chatTabTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Check user role
            //Maybe store a dictionary all viewers who have joined so far
            if (chatClient != null)
            {
                string msg = chatClient.ReadMessage();
                if (!string.IsNullOrEmpty(msg))
                {
                    Console.WriteLine(msg);
                    if (msg.Contains("PRIVMSG"))
                    {
                        DateTime now = DateTime.Now;
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
                                case "!testcmd":
                                    {

                                        apiManager.InitializeSubEvents();
                                    }
                                    break;
                                case "!poll":
                                    {
                                        //Cleanup with sanity checks
                                        if (cmdArgs.Length < 6)
                                        {
                                            chatClient.SendPublicMessage("Command formatted incorrectly. Example: !poll Will_Arco_Win? 2 Yes No 3");
                                            break;
                                        }
                                        string title = cmdArgs[1].Replace('_',' ');
                                        string[] choices = new string[Convert.ToInt32(cmdArgs[2])];
                                        int index = 3;
                                        for (int i =0; i< choices.Length;i++)
                                        {
                                            choices[i] = cmdArgs[3 + i];
                                            index++;
                                        }
                                        var res = await apiManager.CreatePollAsync(title, choices, false, 0, Convert.ToInt32(cmdArgs[index]));
                                        chatClient.SendPublicMessage(res);
                                    }
                                    break;
                                case "!mods":
                                    {
                                        var res = await apiManager.GetModeratorsAsync(); 
                                        chatClient.SendPublicMessage(res);
                                    }
                                    break;
                                case "!followage":
                                    {
                                        string res = "";
                                        if (cmdArgs.Length == 2)
                                            res = await apiManager.GetFollowAgeAsync(cmdArgs[1]);
                                        else
                                            res = await apiManager.GetFollowAgeAsync(userName);
                                        chatClient.SendPublicMessage(res);
                                    }
                                    break;
                                case "!subs":
                                    {
                                        var result = await apiManager.GetSubscriberCountAsync();
                                        chatClient.SendPublicMessage($"{result} members in the Fiesta Fam!");
                                    }
                                    break;
                                case "!randomsub":
                                    {
                                        var result = await apiManager.GetRandomSubAsync();
                                        chatClient.SendPublicMessage(result);
                                    }
                                    break;

                                case "!randomclip":
                                    {
                                        var result = await apiManager.GetRandomClipAsync();
                                        chatClient.SendPublicMessage(result);
                                    }
                                    break;
                                case "!clip":
                                    {
                                        var result = await apiManager.CreateClipAsync();
                                        chatClient.SendPublicMessage(result);
                                    }
                                    break;
                                case "!randomcliphunt":
                                    {
                                        var result = await apiManager.GetClipThumbnail();
                                        chatClient.SendPublicMessage(result);
                                    }
                                    break;
                                case "!sublist":
                                    {
                                        var result = await apiManager.GetSubscriberListAsync();
                                        foreach (var i in result)
                                            Console.WriteLine(i);
                                    }
                                    break;
                                default: chatClient.SendPublicMessage("I don't think that command exists! arcotvSad arcotvSad arcotvSad "); break;
                            }
                        }
                        Dispatcher.Invoke(() => chatTxtBox.AppendText($"[{now.Hour}:{now.Minute}:{now.Second}] {userName}: {submsg}\r\n"));
                    }
                }
            }
        }

        private void StartChatBot()
        {

            chatTabTimer = new System.Timers.Timer(50);
            chatTabTimer.Elapsed += chatTabTimer_Elapsed;

            viewerListTimer = new System.Timers.Timer(60000);
            viewerListTimer.Elapsed += ViewerListTimer_Elapsed;


            chatClient.Start(false);
            chatTabTimer.Start();
            viewerListTimer.Start();

            ViewerListTimer_Elapsed(null, null);

        }

        private async void ViewerListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Separate by viewer type? admin, mod, broadcaster, etc.
            //if count by group is 0, ignore and move to the next
            //if count by group is >= 1, add type to list, then list under.
            var viewerList = await apiManager.GetCurrentViewers();
            await viewertListbox.Dispatcher.BeginInvoke(new Action(() =>
            {
                viewertListbox.IsEnabled = false;
                viewertListbox.Items.Clear();
                foreach (var viewer in viewerList)
                    viewertListbox.Items.Add(viewer);
                viewertListbox.IsEnabled = true;
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0);
        }
        private void chatStartBtn_Click(object sender, RoutedEventArgs e)
        {
            StartChatBot();
        }

        private void chatTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            chatTxtBox.ScrollToEnd();
        }

        private void ircLogTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //ircLogTxtBox.ScrollToEnd();
        }

        private void chatStartBtn_MouseEnter(object sender, MouseEventArgs e)
        {   
            chatStartBtn.Content = FindResource("ConnectTwitchHover");
           
        }

        private void chatStartBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            chatStartBtn.Content = FindResource("ConnectTwitchNormal");

        }
    }
}