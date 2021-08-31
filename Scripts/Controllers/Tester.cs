using System;
using System.Collections.Generic;
using SensorAPI;

public class Tester : IController
{
    public Dictionary<string, DataList> Data { get; private set; }
    public bool IsConnected { get; set; }
    public bool IsUpdating { get; set; }
    public void Start()
    {
    }

    public void Stop()
    {
    }

    public Tester()
    {
        Console.WriteLine("INIT");
    }

    ~Tester()
    {
    }
}
