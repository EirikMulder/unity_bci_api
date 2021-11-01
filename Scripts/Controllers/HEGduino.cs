using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using SensorAPI.Connections;
using SensorAPI.Data;
using SensorAPI.Interfaces;

namespace SensorAPI
{
    namespace Controllers
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

            /**
             * @brief bool: Is true as long as HEG is not returning infinite or invalid numbers
             * These two results are indicitive of incorrect HEGduino setup, or the HEG being badly positioned
             */
            public bool IsStable { get; private set; }
            public bool IsDark { get; private set; }

            // Internal vars
            SerialConnection hegDevice;
            Thread thread;
            bool killThread = false;
            ConcurrentQueue<string> toHEG;
            DateTime lastMessageTime;
            //ConcurrentQueue<string> fromHEG;

            // Constants
            const string StartMsg = "t";
            const string StopMsg = "f";
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
                //fromHEG = new ConcurrentQueue<string>();
                thread = new Thread(ThreadLoop);
                thread.Start();
            }

            /**
             * @brief Stops the HEGduino.
             * Sets IsUpdating to false, kills the management thread, and sends the stop message.
             */
            public void Stop()
            {
                Debug.Log("Stopping...");
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
                Data = new Dictionary<string, DataList>() { { "brain_bloodflow", new DataList() }, { "ambient_light", new DataList() }, { "velocity", new DataList() }, { "acceleration", new DataList() }, { "red_light", new DataList() }, {"ir_light", new DataList() }};
                hegDevice = new SerialConnection(portLocation, BaudRate, readTimeout: 50);
                // TODO: check connection
                IsConnected = true;
            }

            public DataList this[string i]
            {
                get { return Data[i]; }
                set { Data[i] = value; }
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
                while (!killThread || toHEG.Count > 0)
                {
                    if ((DateTime.Now - lastMessageTime).Seconds > 1)
                    {
                        Debug.Log("1s passed, restarting");
                        toHEG.Enqueue(StartMsg);
                        lastMessageTime = DateTime.Now;
                    }
                    if (toHEG.TryDequeue(out string toMessage))
                    {
                        Debug.Log("Thread recieved: " + toMessage);
                        if (!hegDevice.TryWriteLine(toMessage))
                        {
                            IsConnected = false;
                        }
                    }
                    //Debug.Log(toMessage);
                    if (hegDevice.TryReadLine(out string fromMessage))
                    {
                        if (fromMessage != null)
                        {
                            //Debug.Log(fromMessage);
                            lastMessageTime = DateTime.Now;
                            if (fromMessage.Contains("|") && !fromMessage.Contains("inf") && !fromMessage.Contains("nan"))
                            {
                                string[] data = fromMessage.Split('|');
                                Data["red_light"].Add(Convert.ToDouble(data[1]));
                                Data["ir_light"].Add(Convert.ToDouble(data[2]));
                                Data["brain_bloodflow"].Add(Convert.ToDouble(data[3]));
                                Data["ambient_light"].Add(Convert.ToDouble(data[4]));
                                Data["velocity"].Add(Convert.ToDouble(data[5]));
                                Data["acceleration"].Add(Convert.ToDouble(data[6]));
                                IsStable = true;
                            }
                            else if (!fromMessage.Contains("inf") || !fromMessage.Contains("nan"))
                            {
                                IsStable = false;
                            }
                            if (Data["ambient_light"].Latest() > 1200)
                            {
                                IsDark = false;
                            }
                            else
                            {
                                IsDark = true;
                            }
                        }
                        
                    }
                    else
                    {
                        IsConnected = false;
                    }
                }
            }
        }
    } // Namespace Controllers
} // Namespace SensorAPI
