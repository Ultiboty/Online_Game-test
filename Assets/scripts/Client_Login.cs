using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

public class Client_Login : MonoBehaviour
{
    UdpClient udpc;
    IPEndPoint ep;
    Player player;
    bool start_connect = false;
    bool connected = false;
    byte[] rdata;
    byte[] send;
    int fail_count;
    void Start()
    {
        Application.targetFrameRate = 60;
        Login_info.ip = null;
        Login_info.password =null;
        fail_count = 0;
        Debug.Log("started");
    }

    void Update()
    {
        // try connecting to the server
        if (start_connect && !connected)
        {
            if (udpc.Available > 0)
            {
                connected = ReceiveData("hello");
            }
            else
            {
                send = Encoding.ASCII.GetBytes(Login_info.password);
                udpc.Send(send, send.Length);

            }
        }
        // check if info was entered
        if (Login_info.ip != null && Login_info.password != null && !start_connect)
        {
            Debug.Log("got info");
            start_connect = true;
            udpc = new UdpClient(Login_info.ip, 20000);
            ep = null;
        }
        // wait for the server start msg
        if (connected && udpc.Available > 0)
        {
            try
            {
                rdata = udpc.Receive(ref ep);
                player = Deserialize(rdata);
            }
            catch (Exception e)
            {
                return;
            }
            Login_info.client_Player = player;
            Login_info.counter = player.counter;
            Login_info.Client_udpc = udpc;
            SceneManager.LoadScene("Client_Scene");
        }
    }
    bool ReceiveData(string check)
    {
        if (fail_count > 210) // 420 with 60 fps is 7 secs, but we need to account for the fact that we send connection, then check the respond, so 2 iterations for 1 msg.
        {
            Debug.LogError("error reaching the server for 7 sec");
            UnityEditor.EditorApplication.isPlaying = false;
        }
        try
        {
            rdata = udpc.Receive(ref ep);
            if (Encoding.ASCII.GetString(rdata) != check)
            {
                Debug.LogError("Wrong password!");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log("error connecting to the server, error: " + e.Message);
            fail_count++;
            return false;
        }
        fail_count = 0;
        return true;
    }
    static Player Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            IFormatter formatter = new BinaryFormatter();
            return (Player)formatter.Deserialize(memoryStream);
        }
    }
}
