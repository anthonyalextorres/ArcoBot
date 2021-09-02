using CefSharp.Wpf;
using System.Windows;
using Newtonsoft.Json;
using System.Web;
using System.Net.Http;
using System;
using System.Web.UI.HtmlControls;

namespace ArcoBot
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string Code = string.Empty;
        public Login(string uri)
        {
            InitializeComponent();
            setChrome(uri);
            

        }
        private void setChrome(string uri)
        {
            cBrowser.Address = uri;

            cBrowser.AddressChanged += CBrowser_AddressChanged;
        }
        private void getUsername(string uri, CefSharp.IBrowser browser)
        {
            HttpClient client = new HttpClient();
            var resp = client.GetAsync(uri).Result;
            var respContent = resp.Content.ReadAsStringAsync().Result;
            var source = browser.MainFrame.GetSourceAsync().Result;

            var task = browser.MainFrame.EvaluateScriptAsync("(function() { return document.getElementsByClassName('user-info__username')[0].innerText; })();", null).Result;
            string res = task.Result.ToString();
            int endIndex = res.LastIndexOf(@"\\");
            int indexof = @"\nnot you? log out.".Length - 1;
            string name = res.Remove(res.Length - indexof);
           
        }
        private void CBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var browser = (ChromiumWebBrowser)sender;
            var _browser = browser.GetBrowser();

            string uriValue = e.NewValue.ToString();
            if (uriValue.Contains("http://localhost:8080"))//Todo; better parsing and identification :contains(redirect uri)?
            {
                string initVal = e.NewValue.ToString();
                int startIndex = initVal.IndexOf('=') + 1;
                int endIndex = initVal.IndexOf('&');
                string trunVal = initVal.Substring(startIndex, endIndex - startIndex);
                Code = trunVal;
                this.Close();
            }
            else if (uriValue.Contains("https://id.twitch.tv/oauth2/authorize"))
            {
                getUsername(uriValue, _browser);
            }

        }
    }
}
