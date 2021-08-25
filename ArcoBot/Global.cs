using ArcoBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Globally used variables.
/// todo: load from external source.
/// </summary>
public static class Global
{

    public static string OAuth;
    public static string AppAccessToken = default;
    public static string UserAccessToken = default;
    public static string UserAccessRefreshToken = default;
    public static int UserAccessExpiration = default;
    public static string username = "arcotv";
    public static string Channel = "arcotv";
    public static string address = "irc.chat.twitch.tv";
    public static int port = 6667;
    public static Configuration ApplicationConfiguration;
}
