using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class Server_Login : MonoBehaviour
{
    int password;
    Player[] Players;
    IDictionary<IPEndPoint, int> Addresses;
    bool connected = false;
    int counter;
    UdpClient udpc;
    UdpClient host_udpc;
    IPEndPoint ep;
    IPEndPoint ep2;
    byte[] receivedData;
    string received;
    byte[] send;
    // Start is called before the first frame update
    void Start()
    {
        System.Random rnd =new System.Random();
        password = 123456;
        //password = rnd.Next(100000, 1000000);
        Application.targetFrameRate = 60;
        Players = new Player[4];
        Addresses = new Dictionary<IPEndPoint, int>();
        counter = 1;
        udpc = new UdpClient(20000);
        Debug.Log("Server Started and servicing on port no. 7878");
        ep = null;
        ep2 = null;
        host_udpc = new UdpClient("127.0.0.1", 20000);
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!connected)
            {
                Host_Login();
            }
            while (udpc.Available > 0)
            {
                // get data
                receivedData = udpc.Receive(ref ep);
                try
                {
                    received = Encoding.ASCII.GetString(receivedData);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                // if asking player, give
                if (received == password.ToString())
                {
                    if (Addresses.ContainsKey(ep))
                    {
                        SendAgain();
                    }
                    else
                    {
                        AddPlayer();
                    }
                    return;
                }

            }

        }
        catch (Exception e)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.LogError(e.Message + line);
        }
    }
    void Host_Login()
    {
        try
        {
            if (host_udpc.Available > 0)
            {
                // received Data
                //receivedData = udpc.Receive(ref ep2);
                connected = true;
            }
            else
            {
                send = Encoding.ASCII.GetBytes(password.ToString());
                host_udpc.Send(send, send.Length);
            }
            
        }
        catch (Exception e)
        {
            // Get stack trace for the exception with source file information
            var st = new StackTrace(e, true);
            // Get the top stack frame
            var frame = st.GetFrame(st.FrameCount - 1);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Debug.LogError(e.Message + line);
        }
    }
    void SendAgain()
    {
        // send the player that match the id
        Debug.Log("sending again confirmation to player number: " + Addresses[ep]);
        send = Encoding.ASCII.GetBytes("hello");
        udpc.Send(send, send.Length, ep);
    }
    void AddPlayer()
    {
        if (counter > 4)
        {
            Debug.Log("failed connection, too many players");
            return;
        }
        Addresses.Add(ep, counter);
        Players[counter - 1] = new Player(ep, counter);
        Debug.Log("connection from: " + ep + "    assigned player number: " + counter);
        send = Encoding.ASCII.GetBytes("hello");
        udpc.Send(send, send.Length, ep);
        counter++;
    }
    public void StartGame()
    {
        Login_info.Addresses = Addresses;
        Login_info.Players = Players;
        Login_info.counter = counter;
        Login_info.udpc = udpc;
        Login_info.host_udpc = host_udpc;
        Login_info.host_Player = Players[0];
        // send start message
        Debug.Log("starting server");
        for (int j=0;j < 10; j++)
        {
            for (int i = 1; i < counter; i++)
            {
                Players[i - 1].counter = counter;
                send = Serialize(Players[i - 1]);
                udpc.Send(send, send.Length, Players[i-1].ep);
            }
        }
        SceneManager.LoadScene("Host_Scene");
    }
    static byte[] Serialize(object obj)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(memoryStream, obj);
            return memoryStream.ToArray();
        }
    }
}

