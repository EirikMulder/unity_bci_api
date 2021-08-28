using System.IO.Ports;

public class HEGduino : IHEGArduino
{
    bool connected = false;
    SerialPort port;
    const int BAUDRATE = 115200;
    const char STARTMSG = 't';
    const char STOPMSG = 'f';

    public void Connect(string portName)
    {
        try {
            port = new SerialPort(portName, BAUDRATE) {readTimeout = 50};
            connected = true;
        } catch (IOException) {
            throw new SerialDisconnectException("Failed to connect to port");
            connected = false;
        }
    }

    public void Write(string line)
    {
        try {
            port.WriteLine(line);
            port.BaseStream.Flush();
        } catch (IOException) {
            throw new SerialDisconnectException("HEGduino disconnected during write");
            connected = false;
        }
    }

    public string Read()
    {
        string output;
        try {
            output = port.ReadLine();
        } catch (TimeoutException) {
            return null;
        } catch (IOException) {
            throw new SerialDisconnect Exception("HEGduino disconnected during read");
        }
    }

    public void Start()
    {
        Write(STARTMSG);
    }

    public void Stop()
    {
        Write(STOPMSG);
    }

}
