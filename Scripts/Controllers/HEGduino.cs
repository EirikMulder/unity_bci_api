using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using SensorAPI;

namespace SensorAPI
{
/**
 * @brief Controls a single HEGduino device.
 * Connects to and manages a HEGduino device, connected via serial.
 * Provides data output from the device.
 */
public class HEGduino : IController
{
    // Interface
    /**
     * @brief Data output from the device.
     * A dictionary in the format: Label (string): DataList.
     * Use the 'brain_bloodflow' label to access the DataList for bloodflow from the HEG.
     */
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
                fromHEG.Enqueue(fromMessage);
            }
            else
            {
                IsConnected = false;
            }
        }
    }
}
} // Namespace SensorAPI
