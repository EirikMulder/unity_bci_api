using System.Collections.Generic;
using System;

public class Tester : IController
{
    public Dictionary<string, DataList> data { get; private set; }
    public bool isConnected { get; set; }
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
