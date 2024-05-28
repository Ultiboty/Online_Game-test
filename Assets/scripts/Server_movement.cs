using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Server_movement : MonoBehaviour
{
    private Rigidbody2D rb;
    Player my_player;
    bool connected = false;
    //network vars
    public string ip = "";
    UdpClient udpc;
    IPEndPoint ep;
    byte[] send;
    byte[] rdata;
    string bp;

    // Start is called before the first frame update
    void Start()
    {
        my_player = Login_info.host_Player;
        Application.targetFrameRate = 60;
        udpc = Login_info.host_udpc;
        ep = null;
    }

    // Update is called once per frame
    void Update()
    {
        try
        { 
            GetInput();
            // send data
            send = Serialize(my_player);
            udpc.Send(send, send.Length);
           
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
    void GetInput()
    {
        // get horizontal movement
        my_player.dirX = Input.GetAxis("Horizontal");

        // get jump
        my_player.jump = 0;
        if (Input.GetButtonDown("Jump"))
        {
            my_player.jump = 1;
        }

        // get side to look
        if (my_player.dirX > 0)
        {
            my_player.rotation = 0;
        }
        else if (my_player.dirX < 0)
        {
            my_player.rotation = 180;
        }
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
    // Deserialize a byte array into an object
    static Player Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            IFormatter formatter = new BinaryFormatter();
            return (Player)formatter.Deserialize(memoryStream);
        }
    }
}
