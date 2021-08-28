using System.IO.Ports;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.IO;
using System;
using System.Diagnostics;
using UnityEngine;

public class HEGduino : BCISensor
{
    // Data
    [Header("Data")]
    public double score = 0;
    public double weightedScore = 0;
    public double rawScore = 0;
    public bool isUnstable;
    public long time;
    double scoreAccumulator;
    Queue<double> scoreBuffer;
    List<double> scoreBufferList = new List<double>();

    // Port Info
    SerialPort port;
    fakeHEG fakePort;

    // Threading
    Thread thread;
    ConcurrentQueue<string> toThread;
    ConcurrentQueue<string> fromThread;

    // Constants
    const int BAUDRATE = 115200;
    const string STARTMSG = "t";
    const string STOPMSG = "f";
    [Header("Config")]
    public int bufferSize = 120;
    public double maxDeviation = 1;

    // TODO: TESTING
    
    Stopwatch st = new Stopwatch();
    [Header("Debug")]
    public bool debugMessages = false;
    public double smoothTime = 0;
    public double weightedTime = 0;
    public int readings = 0;
    public bool useFakeHEG = false;

    // Standard Behavior
    public void Update()
    {
        if (connected && fromThread.Count != 0)
        {
            bool success;
            success = fromThread.TryDequeue(out string rawData);
            print(rawData);
            if (success && (rawData.Contains("nan") || rawData.Contains("inf") || !rawData.Contains("|")))
                return;
            double rawScore = Convert.ToDouble(rawData.Split('|')[3]);
            this.rawScore = rawScore;
            double ambientLight = Convert.ToDouble(rawData.Split('|')[4]);
            if (ambientLight > 1100)
                isUnstable = true;
            else
                isUnstable = false;
            time = Convert.ToInt64(rawData.Split('|')[0]);
            if (Math.Abs(rawScore - score) > maxDeviation)
                rawScore = maxDeviation;
            st.Restart();
            scoreBuffer.Enqueue(rawScore);
            score = Smooth(rawScore);
            st.Stop();
            smoothTime = st.ElapsedMilliseconds * 1000.00;
            st.Restart();
            weightedScore = WeightedSmooth(rawScore);
            st.Stop();
            weightedTime = st.ElapsedMilliseconds * 1000.00;
            ++readings;

        }
    }

    double Smooth(double rawScore)
    {
        if (scoreBuffer.Count <= bufferSize)
        {
            scoreAccumulator += rawScore;
        }
        else
        {
            scoreAccumulator -= (double)scoreBuffer.Dequeue();
            scoreAccumulator += rawScore;
        }
        return scoreAccumulator / scoreBuffer.Count;
    }

    double WeightedSmooth(double rawScore)
    {
        scoreBufferList.Add(rawScore);
        if (scoreBufferList.Count > bufferSize)
            scoreBufferList.RemoveAt(0);
        double average = 0;
        int length = scoreBufferList.Count;
        for(int i = 0; i < length; ++i)
        {
            average += scoreBufferList[i] * (i + 1);
        }
        average /= length * (length + 1) / 2;
        return average;
    }

    // Connections
    // Connect to serial port at portName.
    public bool Connect(string portName)
    {
        if (debugMessages) st.Restart();
        print("Running Connect() with " + portName);
        scoreBuffer = new Queue<double>();
        try
        {
            if (useFakeHEG)
                port = new fakeHEG(portName, BAUDRATE) { ReadTimeout = 50 };
            else
                port = new SerialPort(portName, BAUDRATE) { ReadTimeout = 50 };
            port.Open();
            connected = true;
            StartThread();
            Thread.Sleep(5000);
            toThread.Enqueue(STARTMSG);
            print("Connected.");
        }
        catch (IOException)
        {
            Disconnect("Connection Failed");
            connected = false;
        }
        if (debugMessages) st.Stop();
        if (debugMessages) print(st.ElapsedMilliseconds / 1000.00 + " Microseconds for connection");
        
        return connected;
    }

    // Takes a disconnect message and sets connected to false.
    void Disconnect(string message)
    {
        print(message);
        connected = false;
        try
        {
            port.WriteLine(STOPMSG);
            port.BaseStream.Flush();
            port.Close();
        } catch (IOException)
        {
            return;
        }
    }

    public override void Destroy()
    {
        Disconnect("Game Over");
    }

    // Threading
    void StartThread()
    {
        print("Running StartThread()");
        toThread = new ConcurrentQueue<string>();
        fromThread = new ConcurrentQueue<string>();
        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    // Thread target: loop reading and writing to the HEGduino.
    void ThreadLoop()
    {
        while (connected && port.IsOpen)
        {
            Stopwatch sw = new Stopwatch();
            if (debugMessages) sw.Restart();
            try {
                if (toThread.Count != 0)
                {
                    print("Writing to HEG");
                    // Write command to HEGduino.
                    toThread.TryDequeue(out string message);
                    port.WriteLine(message);
                    port.BaseStream.Flush();
                }
                try {
                    // Read from port into fromThread Queue.
                    if (debugMessages) st.Stop();
                    if (debugMessages) print(st.ElapsedMilliseconds / 1000.00 + " Microseconds for timeout");
                    
                    fromThread.Enqueue(port.ReadLine());
                } catch (TimeoutException) {}
            } catch (IOException) {
                Disconnect("Lost Connection during I/O");
                break;
            }
            if (debugMessages) sw.Stop();
            if (debugMessages) print(sw.ElapsedMilliseconds/1000.00 + " Microseconds for read");
            
        }
    }
}
