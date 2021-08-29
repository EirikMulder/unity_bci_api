using System;
using System.IO;
using System.IO.Ports;

namespace SensorAPI
{
public class SerialConnection
{
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
    public int messageDelay = 5000;

    SerialPort port;

    public SerialConnection(string portName, int baudRate, int readTimeout = 50)
    {
        port = new SerialPort(portName, baudRate);
        this.ReadTimeout = readTimeout;
    }

    public bool TryWriteLine(string message)
    {
        try
        {
            port.WriteLine(message);
            port.BaseStream.Flush();
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

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
} // Namespace SensorAPI
