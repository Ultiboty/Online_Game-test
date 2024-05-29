using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

[Serializable]
public class Player
{
    public IPEndPoint ep { get; set; }
    public int id { get; set; }
    public int counter { get; set; }
    public int jump { get; set; }
    public float dirX { get; set; }
    public float[] position { get; set; }
    public float rotation { get; set; }
    public Player(IPEndPoint ep, int id)
    {
        this.ep = ep;
        this.id = id;
        jump = 0;
        dirX = 0;
        position = new float[2] { 0, 0 };
        rotation = 0;
    }
}
