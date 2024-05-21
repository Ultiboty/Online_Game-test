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

public class Client : MonoBehaviour
{
    private Rigidbody2D rb;
    Player player;
    Player my_player;
    bool connected = false;
    //network vars
    public string ip = "";
    UdpClient udpc;
    IPEndPoint ep;
    byte[] send;
    byte[] rdata;
    string bp;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        udpc = new UdpClient("192.168.1.154", 7878);
        ep = null;
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (!connected)
            {
                if (udpc.Available > 0)
                {
                    // received Data
                    rdata = udpc.Receive(ref ep);
                    my_player = Deserialize(rdata);
                    connected = true;
                }
                else
                {
                    send = Encoding.ASCII.GetBytes("hello");
                    udpc.Send(send, send.Length);
                }
            }
            else
            {
                GetInput();

                // send data
                send = Serialize(my_player);
                udpc.Send(send, send.Length);

                // for each player sent to us, move
                while (udpc.Available > 0)
                {
                    HandlePlayer();
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
    void HandlePlayer()
    {
        // receive Data
        rdata = udpc.Receive(ref ep);
        player = Deserialize(rdata);

        // find player
        rb = GameObject.Find("player" + player.id).GetComponent<Rigidbody2D>();

        // move to current position
        rb.transform.position = new Vector3(player.position[0], player.position[1], 0);

        // rotate to side looking
        rb.transform.rotation = Quaternion.Euler(0, player.rotation, 0);

        // jump
        if (player.jump == 1)
            rb.velocity = new Vector2(rb.velocity.x, 13f);

        // move hirzontaly
        rb.velocity = new Vector2(player.dirX * 7f, rb.velocity.y);

        // running animation
        anim = GameObject.Find("player" + player.id).GetComponent<Animator>();
        anim.SetBool("running" + player.id, player.dirX != 0);
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

