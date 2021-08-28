using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class HEGduino : IController
{
    public Dictionary<string, DataList> data { get; private set; }
    public bool isConnected { get; private set; }
    public bool isUpdating { get; private set; }

    SerialPort serialPort;
    Thread thread;
    string portLocation;
    bool killThread = false;
    ConcurrentQueue<string> toHEG;
    ConcurrentQueue<string> fromHEG;

    const string STARTMSG = "f";
    const string STOPMSG = "t";
    const int BAUDRATE = 115200;

    public void Start()
    {
        killThread = false;
        isUpdating = true;
        toHEG = new ConcurrentQueue<string>();
        fromHEG = new ConcurrentQueue<string>();
        thread = new Thread(ThreadLoop);
        thread.Start();
        toHEG.Enqueue(STARTMSG);
    }

    public void Stop()
    {
        isUpdating = false;
        if (thread.IsAlive)
        {
            toHEG.Enqueue(STOPMSG);
            killThread = true;
        }
    }

    public HEGduino(string portName)
    {
        data = new Dictionary<string, DataList>() {{ "brain_bloodflow", new DataList() }};
        this.portName = portName;
    }

    ~HEGduino()
    {
        if (isConnected)
        {
            Stop();
            isConnected = false;
        }
    }

    void ThreadLoop()
    {
        while (!killThread)
        {
            try
            {
                if (toHEG.Count > 0)
                {
                    toHEG.TryDequeue(out string message);
                    serialPort.WriteLine(message);
                    serialPort.BaseStream.Flush();
                }
                try
                {
                    fromHEG.Enqueue(serialPort.ReadLine());
                }
                catch (TimeoutException) {}
            }
            catch (IOException)
            {
                isConnected = false;
                killThread = true;
                break;
            }
        }
    }
}
