using ArcoBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArcoBot.Interfaces
{
    public interface IToken
    {
        HttpClient Client { get; }
        Thread Worker { get; }
        string AccessToken { get; }
        DateTime Expiration { get; }
        string[] Scopes { get; }
        void CheckToken();
    }
}