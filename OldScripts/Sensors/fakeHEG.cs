using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;
using System.IO.Ports;

[Serializable]
public class fakeHEG : SerialPort
{
    Queue<string> outBuffer;
    Queue<string> inBuffer;
    double lastValue = 0;
    double lastTime;
    int time = 0;
    bool running = false;
    bool increasing = true;
    Thread thread;

    new public int ReadTimeout { get; internal set; }

    public fakeHEG(string portName, int baudRate)
    {
        outBuffer = new Queue<string>();
        inBuffer = new Queue<string>();
        thread = new Thread(ThreadLoop);
    }

    new public void Open()
    {
        running = true;
    }

    new public void Close()
    {
        running = false;
    }

    new public void WriteLine(string message)
    {
        inBuffer.Enqueue(message);
    }

    new public string ReadLine()
    {
        return outBuffer.Dequeue();
    }

    public string GenerateNumber()
    {
        time += 61000;
        if (lastValue > 5) { increasing = false; }
        if (lastValue < 5) { increasing = true; }
        double value = 0;
        if (increasing)
        {
            value = lastValue + 0.2;
        }
        if (!increasing)
        {
            value = lastValue - 0.2;
        }
        Debug.Log("RETURNING MESSAGE");
        return time + "|1000|1000|" + value + "|800";
    }

    void ThreadLoop()
    {
        while (true)
        {
            if (outBuffer.Count != 0)
            {
                string x = outBuffer.Dequeue();
                if (x == "t")
                    running = true;
                else
                    running = false;
            }
                
            if (running)
            {
                if (Time.time > lastTime + 0.061)
                {
                    outBuffer.Enqueue(GenerateNumber());
                    lastTime = Time.time;
                }
            }
        }
    }

    new public class BaseStream
    {
        public void Flush()
        {
        }
    }
}
