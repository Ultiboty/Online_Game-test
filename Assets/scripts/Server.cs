using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;
using Debug = UnityEngine.Debug;


public class Server : MonoBehaviour
{
    //game vars
    Player player;
    private Rigidbody2D rb;
    //network vars
    Player[] Players;
    IDictionary<IPEndPoint, int> Addresses;
    int counter;
    string address;
    UdpClient udpc;
    IPEndPoint ep;
    byte[] receivedData;
    string received;
    byte[] send;
    Animator anim;
    void Start()
    {
        Application.targetFrameRate = 60;
        Players = Login_info.Players;
        Addresses = Login_info.Addresses;
        counter = Login_info.counter;
        udpc = Login_info.udpc;
        Debug.Log("started playing");
        ep = null;
        for (int i = 4; i > counter-1; i--)
        {
            GameObject.Find("player" + i).SetActive(false);
        }
    }


    void Update()
    {
        try
        {
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
             
                //move based on data
                player = Deserialize(receivedData);
                rb = GameObject.Find("player" + player.id).GetComponent<Rigidbody2D>();
                player.position[0] = rb.transform.position.x;
                player.position[1] = rb.transform.position.y;
                Players[player.id - 1] = player;
                MovePlayer();

                // send all players to client
                for (int i = 1; i < counter; i++)
                {
                    send = Serialize(Players[i - 1]);
                    udpc.Send(send, send.Length, ep);
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
    void MovePlayer()
    {
        // jump
        if (player.jump == 1)
        {
            rb.velocity = new Vector2(rb.velocity.x, 13f);
            Debug.Log(player.id + "has jumped: " + player.jump);
        }

        // rotate to side looking
        rb.transform.rotation = Quaternion.Euler(0, player.rotation, 0);

        // move horizontaly
        rb.velocity = new Vector2(player.dirX * 7f, rb.velocity.y);

        // set animation
        anim = GameObject.Find("player" + player.id).GetComponent<Animator>();
        anim.SetBool("running", player.dirX != 0);
        //anim.SetBool("jump", player.jump != 0);
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