using System;
using System.IO;
using System.IO.Ports;
using UnityEngine;
using System.Threading;

namespace SensorAPI
{
namespace Connections
{
/**
 * @brief Manages a serial connection
 */
public class SerialConnection
{
    /** 
     * @brief Duration TryReadLine will wait before returning null.
     */
    public int ReadTimeout {
        get
        {
            return port.ReadTimeout;
        }
        set
        {
            port.ReadTimeout = ReadTimeout;
        }
    }

    /**
     * @brief TryWriteLine: The delay in milliseconds after writing a message to the port before returning.
     */
    public int messageDelay = 5000;

    SerialPort port;

    /**
     * @brief Initializes and opens the serial port.
     * @param portName String: location of the port.
     * @param baudRate int: Baud Rate of the serial port.
     * @param readTimeout int: Delay before returning null on a read.
     */
    public SerialConnection(string portName, int baudRate, int readTimeout = 50)
    {
                port = new SerialPort(portName, baudRate)
                {
                    ReadTimeout = readTimeout
                };
                port.Open();
    }

    /**
     * @brief Writes a string to the serial port, and returns true/false for success.
     * @param message String: the message to write to the port.
     */
    public bool TryWriteLine(string message)
    {
        try
        {
            port.WriteLine(message);
            port.BaseStream.Flush();
            Debug.Log("Wrote: " + message);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    /**
     * @brief Attempts to read from the serial port into the output string message. Returns a bool indicitive of the success.
     * @param message out string: The message that is recieved, null if nothing returned.
     */
    public bool TryReadLine(out string message)
    {
        try
        {
            try
            {
                message = port.ReadLine();
            }
            catch (TimeoutException)
            {
                message = null;
                return true;
            }
            port.BaseStream.Flush();
            return true;
        }
        catch (IOException)
        {
            message = null;
            return false;
        }
    }
}
} // Namespace Connections
} // Namespace SensorAPI
