using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using SensorAPI;

namespace SensorAPI
{
public class HEGduino : IController
{
    // Interface
    public Dictionary<string, DataList> Data { get; private set; }
    public bool IsConnected { get; private set; }
    public bool IsUpdating { get; private set; }

    // Internal vars
    SerialConnection hegDevice;
    Thread thread;
    string portLocation;
    bool killThread = false;
    ConcurrentQueue<string> toHEG;
    ConcurrentQueue<string> fromHEG;

    // Constants
    const string StartMsg = "f";
    const string StopMsg = "t";
    const int BaudRate = 115200;

    public void Start()
    {
        killThread = false;
        IsUpdating = true;
        toHEG = new ConcurrentQueue<string>();
        fromHEG = new ConcurrentQueue<string>();
        thread = new Thread(ThreadLoop);
        thread.Start();
        Thread.Sleep(2000);
        toHEG.Enqueue(StartMsg);
    }

    public void Stop()
    {
        IsUpdating = false;
        if (thread.IsAlive)
        {
            toHEG.Enqueue(StopMsg);
            killThread = true;
        }
    }

    public HEGduino(string portLocation)
    {
        Data = new Dictionary<string, DataList>() {{ "brain_bloodflow", new DataList() }};
        this.portLocation = portLocation;
        hegDevice = new SerialConnection(portLocation, BaudRate, readTimeout: 50);
        // TODO: check connection
        IsConnected = true;
    }

    ~HEGduino()
    {
        if (IsConnected)
        {
            Stop();
            IsConnected = false;
        }
    }

    void ThreadLoop()
    {
        while (!killThread)
        {
            if (toHEG.TryDequeue(out string toMessage))
            {
                if (!hegDevice.TryWriteLine(toMessage))
                {
                    IsConnected = false;
                }
            }
            if (hegDevice.TryReadLine(out string fromMessage))
            {
                //fromHEG.Enqueue(fromMessage);
                string[] data = fromMessage.Split('|');
                Data["brain_bloodflow"].Add(Convert.ToDouble(data[3]));
                Debug.Log(data);
            }
                else
            {
                IsConnected = false;
            }
        }
    }
}
} // Namespace SensorAPI
