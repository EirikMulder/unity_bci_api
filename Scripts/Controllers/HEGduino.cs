using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
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
    /**
     * @brief Data output from the device.
     * A dictionary in the format: Label (string): DataList.
     * Use the 'brain_bloodflow' label to access the DataList for bloodflow from the HEG.
     */
    public Dictionary<string, DataList> Data { get; private set; }
    /**
     * @brief bool: Is the device connected to the serial port.
     */
    public bool IsConnected { get; private set; }
    /**
     * @brief bool: Is the data field refreshing (false if device is connected but disabled)
     */
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

    /**
     * @brief Starts the HEGduino.
     * Sets IsUpdating to true, launches the management thread, and sends the start message.
     */
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

    /**
     * @brief Stops the HEGduino.
     * Sets IsUpdating to false, kills the management thread, and sends the stop message.
     */
    public void Stop()
    {
        IsUpdating = false;
        if (thread.IsAlive)
        {
            toHEG.Enqueue(StopMsg);
            killThread = true;
        }
    }

    /**
     * @brief Initializes the HEG connection with the provided serial port.
     * Initializes the Data dictionary, sets up the serial connection, sets IsConnected to true.
     * @param portLocation A string with the serial port location (COMx on windows, file location on linux/macos).
     */
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
