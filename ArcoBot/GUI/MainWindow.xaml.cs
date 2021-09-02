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
        public System.Timers.Timer chatTabTimer;
        public System.Timers.Timer lurkerTabTimer;
        public System.Timers.Timer viewerListTimer;
        public ApiManager apiManager;
        RichTextBoxOutputter outputter;

        public MainWindow()
        {
            InitializeComponent();




            // Enter the listening loop.
            //outputter = new RichTextBoxOutputter(chatTxtBox);
            // Console.SetOut(outputter);
            //Console.WriteLine("Bot has initiated.");
        }
        private async void chatTabTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            string msg = await apiManager?.ReadMessage();

            if (msg != default)
                Dispatcher.Invoke(() => chatRichTxtBox.AppendText($"{msg}\r\n"));
        }

        private async void ViewerListTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Separate by viewer type? admin, mod, broadcaster, etc.
            //if count by group is 0, ignore and move to the next
            //if count by group is >= 1, add type to list, then list under.
            var viewerList = await apiManager.GetCurrentViewers();
            await viewerListbox.Dispatcher.BeginInvoke(new Action(() =>
            {
                viewerListbox.IsEnabled = false;
                viewerListbox.Items.Clear();
                foreach (var viewer in viewerList)
                    viewerListbox.Items.Add(viewer);
                viewerListbox.IsEnabled = true;
            }));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatStartBtn_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatTxtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //chatTxtBox.ScrollToEnd();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chatStartBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            //chatStartBtn.Content = FindResource("ConnectTwitchHover");

        }

        private void chatStartBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            //chatStartBtn.Content = FindResource("ConnectTwitchNormal");

        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void viewerListBoxVisit_Click(object sender, RoutedEventArgs e)
        {
            if (viewerListbox.SelectedIndex == -1) return;//Open via chromium?
            System.Diagnostics.Process.Start($"https://twitch.tv/{viewerListbox.SelectedValue}");

        }
        private async void viewerListBoxBan_Click(object sender, RoutedEventArgs e)
        {
            await apiManager.Ban(Convert.ToString(viewerListbox.SelectedValue));
        }

        private void connectBtn_Click(object sender, RoutedEventArgs e)
        {
            apiManager = new ApiManager();
            if (!apiManager.Connected)
            {
                MessageBox.Show("Error connecting");
                return;
            }
            chatTabTimer = new System.Timers.Timer(50);
            chatTabTimer.Elapsed += chatTabTimer_Elapsed;

            viewerListTimer = new System.Timers.Timer(60000);
            viewerListTimer.Elapsed += ViewerListTimer_Elapsed;

            chatTabTimer.Start();
            viewerListTimer.Start();

            ViewerListTimer_Elapsed(null, null);

            foreach (TabItem tabItem in tabControl.Items)
            {
                var tabString = tabItem.Header.ToString();
                if (tabString != "Initialize")
                {
                    tabItem.Visibility = Visibility.Visible;
                    continue;
                }
            }
            tabControl.Items.Remove(tabControl.SelectedItem);
            tabControl.SelectedIndex = tabControl.SelectedIndex + 1;
        }

        private void sendChatMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(chatMsgTxtBox.Text))
            {
                apiManager.SendPublicMessage(chatMsgTxtBox.Text);
                chatMsgTxtBox.Clear();
            }
        }

        private void chatMsgTxctBox_onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendChatMsgBtn_Click(null, null);
            }
        }
    }
}