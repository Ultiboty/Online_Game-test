using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static class Login_info 
{
    public static string ip { get; set; }
    public static string password { get; set; }
    public static Player[] Players { get; set; }
    public static IDictionary<IPEndPoint, int> Addresses { get; set; }
    public static int counter { get; set; }
    public static UdpClient udpc { get; set; }
    public static UdpClient host_udpc { get; set; }
    public static UdpClient Client_udpc { get; set; }
    public static Player client_Player { get; set; }
    public static Player host_Player { get; set; }
}
