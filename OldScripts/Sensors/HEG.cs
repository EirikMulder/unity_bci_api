using UnityEngine;
using System.IO.Ports;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class HEGduino : BCISensor
{
    // Status
    bool connected {get;}

    public double score = 0;

    IHEGArduino heg;

    // Threading
    Thread thread;
    ConcurrentQueue<string> toThread;
    ConcurrentQueue<string> fromThread;

    // Constants
    const int BUFFER = 12;
    const decimal MAXDEVIATION = 5;

    // Constructors
    [InjectionConstructor]
    public HEGduino(IHEGArduino heg)
    {
        this.heg = heg;
    }

    public HEGduino()
    {
        throw new NotImplementedError("Automatic HEG Constructor is not available.");
    }

    // Standard Behavior
    public void Update()
    {
        if (fromThread.Count != 0)
        {
            string rawData = fromThread.Dequeue();
            if (rawData.Contains("nan") || rawData.Contains("inf"))
                return;
            double rawScore = (double)rawData.Split('|')[3];
            if (abs(rawScore - score) > MAXDEVIATION)
                rawScore = MAXDEVIATION;
            scoreBuffer.Enqueue(rawScore);
            score = Smooth(rawScore);
            rawScore.Dispose();
        }
    }

    // Connections
    // Connect to serial port at portName.
    void Connect()
    {
        try {
            heg.Connect
            connected = true;
        } catch (SerialDisconnectException) {
            Disconnect("Connection Failed");
        }
        return connected;
    }

    // Takes a disconnect message and sets connected to false.
    void Disconnect(string message)
    {
        connected = false;
        print(message);
    }

    // Threading
    void StartThread()
    {
        toThread = new ConcurrentQueue();
        fromThread = new ConcurrentQueue();
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    // Thread target: loop reading and writing to the HEGduino.
    void ThreadLoop()
    {
        while (true)
        {
            try {
                if (toThread.Count != 0)
                {
                    // Write command to HEGduino.
                    port.WriteLine(toThread.Dequeue);
                    port.BaseStream.Flush();
                }
                try {
                    // Read from port into fromThread Queue.
                    fromThread.Enqueue(port.ReadLine());
                } catch (TimeoutException) {}
            } catch (IOException) {
                Disconnect("Lost Connection during I/O");
                break;
            }
        }
    }
}
