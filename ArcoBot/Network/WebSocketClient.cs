using ArcoBot.JsonObjects;
using ArcoBot.Network.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot.Network
{
    /// <summary>
    /// 
    /// </summary>
    public class WebSocketClient

    {
        /// <summary>
        /// Client used to communicate with websocket endpoint.
        /// </summary>
        private readonly ClientWebSocket client;

        /// <summary>
        /// Returns whether or not the client's connection is currently open.
        /// </summary>
        private bool Connected => client?.State == WebSocketState.Open;

        /// <summary>
        /// PLEASE LEARN ABOUT THIS
        /// </summary>
        private CancellationTokenSource tokenSource;
        
        /// <summary>
        /// Returns tokenSource's current token.
        /// </summary>
        private CancellationToken token => tokenSource.Token;

        /// <summary>
        /// URI for the client to connect to.
        /// </summary>
        private Uri uri;

        /// <summary>
        /// Handles data received from the websocket endpoint.
        /// </summary>
        public event EventHandler<OnMessageEventArgs> OnMessage;


        /// <summary>
        /// Constructor.
        /// </summary>
        public WebSocketClient()
        {
            client = new ClientWebSocket();
            tokenSource = new CancellationTokenSource();
            uri = new Uri("wss://pubsub-edge.twitch.tv");
        }

        /// <summary>
        /// Opens the connection to the websocket endpoint.
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            if (Connected) return true;

            client.ConnectAsync(uri, token).Wait(10000);
            if (!Connected)
                return Open();

            return true;
        }
        
        /// <summary>
        /// Listens for messages received from the websocket endpoint.
        /// </summary>
        /// <returns></returns>
        public Task Listen()
        {

            return Task.Run(async () =>
               {
                   var message = "";

                   while (Connected)
                   {
                       WebSocketReceiveResult result;
                       var buffer = new byte[1024];

                       result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), tokenSource.Token);
                       if (result == null) continue;

                       switch (result.MessageType)
                       {
                           case WebSocketMessageType.Close:
                               //  Close();
                               break;
                           case WebSocketMessageType.Text when !result.EndOfMessage:
                               message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                               continue;
                           case WebSocketMessageType.Text:
                               message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                               OnMessage?.Invoke(this, new OnMessageEventArgs() { Message = message });
                               break;
                           case WebSocketMessageType.Binary:
                               break;
                           default:
                               throw new ArgumentOutOfRangeException();
                       }

                       message = string.Empty;


                   }
               });
        }
        /// <summary>
        /// Sends data from the client to the websocket endpoint.
        /// </summary>
        /// <param name="msg">Data to send to websocket endpoint.</param>
        /// <returns></returns>
        public async Task Send(string msg)
        {
            await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), WebSocketMessageType.Text, true, tokenSource.Token);
        }
    }
}
